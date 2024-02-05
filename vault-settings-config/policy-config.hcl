path "secret*" {
    capabilities = ["read","list"]
},
path "sys/mounts"
{
  capabilities = ["read","list"]
}
