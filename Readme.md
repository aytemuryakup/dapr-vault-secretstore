# A simple dapr secret management project with golang

## Prerequisites

- [Hashicorp Vault](https://developer.hashicorp.com/vault/docs/platform/k8s/helm)
- [Kubernetes](https://kubernetes.io/docs/setup/) minikube can be used


## Run the app on Kubernetes

## Install Dapr

```bash
kubectl create ns dapr-vault
# Add Dapr Repo
helm repo add dapr https://dapr.github.io/helm-charts/

# Install Dapr
helm upgrade --install dapr dapr/dapr \
--version=1.12 \
--namespace dapr-vault \
--wait

# Verify Dapr
kubectl get pods --namespace dapr-vault
```

## Install Vault
```bash
# Add Hashicorp Vault Repo
helm repo add hashicorp https://helm.releases.hashicorp.com

# Install Hashicorp Vault

helm upgrade --install vault hashicorp/vault \
--version=0.25.0 \
--namespace dapr-vault \
--wait

# Verify Hashicorp Vault
kubectl get pods --namespace dapr-vault
```

### Configure Vault
```bash
# Initialize Vault Server
kubectl exec -n dapr-vault --stdin=true --tty=true vault-0 -- vault operator init

# Unseal the Vault server
kubectl exec -n dapr-vault --stdin=true --tty=true vault-0 -- vault operator unseal # key-1
kubectl exec -n dapr-vault --stdin=true --tty=true vault-0 -- vault operator unseal # key-2
kubectl exec -n dapr-vault --stdin=true --tty=true vault-0 -- vault operator unseal # key-3

vault login
vault secrets enable -path=secret kv-v2
vault kv put secret/dapr/go-secret-lab/do-not-use-vault-token do-not-use-vault-token="nice work!!!"
```