# PaymentGateway

An API to enable merchants to offer a way for their shoppers to pay for their product.

## Run

Install [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

Then run

```sh
dotnet run -p src

# or with hot-reloading
dotnet watch -p src run
```

Browse to https://localhost:5001/healthcheck or https://localhost:5001/swagger

## Run in K8S

Install Docker Desktop and enable Kubernetes

Install [skaffold](https://skaffold.dev/docs/install/) (tested on v1.11.0)

Then run note that `port-forward` switch is to expose `NodePort` service on localhost

   ```sh
   skaffold run

   # for hot reloading
   skaffold dev
   ```

Browse to http://localhost:5001/healthcheck or http://localhost:5001/swagger

## Run tests

```sh
# run unit tests
dotnet test tests.unit

# run integration tests
dotnet test tests.integration
```
