## What to do for Vault dynamic token provisioning

```bash
# We exec and login to the Vault pod.
kubectl exec -n dapr-vault --stdin=true --tty=true vault-0 -- /bin/sh
vault login # with token

vault secrets enable -path=secret kv-v2
vault kv put secret/dapr/go-secret-lab/do-not-use-vault-token do-not-use-vault-token="nice work!!!"

# we deploy vault read and list policy to vault.

vault policy write dapr-vault-read-token-policy - << EOF
path "secret*" {
    capabilities = ["read","list"]
}
path "sys/mounts"
{
  capabilities = ["read","list"]
}
EOF

# We are activating the vault auth method k8s.
vault auth enable -path=dapr-vault-agent-method kubernetes

# We write vault auth method.
vault write auth/dapr-vault-agent-method/config \
      kubernetes_host="https://$KUBERNETES_PORT_443_TCP_ADDR:443" \
      token_reviewer_jwt="$(cat /var/run/secrets/kubernetes.io/serviceaccount/token)" \
      kubernetes_ca_cert=@/var/run/secrets/kubernetes.io/serviceaccount/ca.crt


# We are writing role for vault auth method.
vault write auth/dapr-vault-agent-method/role/dapr-vault-agent-role \
   bound_service_account_names=dapr-vault-admin \
   bound_service_account_namespaces=dapr-vault-lab \
   policies=dapr-vault-read-token-policy \
   ttl=5d

kubectl apply -f vault-settings-config/vault-application-config.yaml
```

**NOTE:** If Vault and the application you developed will be in different kubernetes clusters, you need to change the _ClusterRole_ and _kubernetes method_.
```bash
# The following rbac file should be deployed to the application cluster.
kubectl apply -f vault-settings-config/rbac.yaml

# Service Account token must be created.

# auth method should be changed.
vault write auth/dapr-vault-agent-method/config kubernetes_host="$KUBE_API_SERVER_URL" token_reviewer_jwt="$SERVICE_ACCOUNT_TOKEN" kubernetes_ca_cert="$KUBERNETES_CA_FILE"
```
