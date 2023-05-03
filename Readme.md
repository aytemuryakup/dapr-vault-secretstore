# A simple dapr secret management project with dotnet

## Prerequisites

- [.NET 5+](https://dotnet.microsoft.com/download) 
- [Dapr .NET SDK](https://docs.dapr.io/developing-applications/sdks/dotnet/)
- [Hashicorp Vault](https://developer.hashicorp.com/vault/docs/platform/k8s/helm)
- [Kubernetes](https://kubernetes.io/docs/setup/) minikube can be used

## Overview
The Secrets management building block in Dapr currently has a few different options for accessing the store from a .NET application:
* HTTP
* Using the DaprClient in the Dapr .NET SDK
* Using the configuration provided in the .NET SDK

The first two are fairly straight forward and the third is only slightly more complex, but it adds the benefits of tying into the existing .NET configuration system and allowing for dependency injection. All three are demonstrated in the Startup.cs file using a local secret store located at /components/secretstore-lab.json and defined by /components/secretstore.yaml.

## Run the app on Kubernetes