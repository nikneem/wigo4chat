# Copilot Instructions: ASP.NET Web API with .NET 9, Aspire, and Dapr

## Solution Overview

- Create an ASP.NET Web API project targeting .NET 9.
- Use .NET Aspire for cloud-native orchestration and configuration.
- Store all Aspire-related projects in a folder named `Aspire`.
- Integrate Dapr for distributed application runtime features.
- Configure two Dapr components:
  - **State Store**: Uses Redis as the backend for storage.
  - **Pub/Sub**: Uses Redis as the backend for publish/subscribe messaging.

## Project Structure

```
/src/SolutionRoot
  /Aspire
    (All Aspire projects and configuration)
  /Chat
    (ASP.NET Web API project)
    (Chat service class library)
    (Users service class library)
  /Shared
    (A shared library with common used code)
/dapr
  (Dapr component YAML files)
```

## Steps

1. **Create the ASP.NET Web API project** targeting .NET 9.
2. **Add .NET Aspire orchestration**:
   - Place Aspire projects inside the `Aspire` folder.
   - Configure Aspire to orchestrate the Web API and Dapr sidecars.
3. **Add Dapr support**:
   - Reference Dapr packages in the Web API project.
   - Configure the Web API to communicate with Dapr sidecars.
4. **Configure Dapr components**:
   - Create a `dapr` folder at the solution root.
   - Add two YAML files:
     - `statestore.yaml` for Redis state store.
     - `pubsub.yaml` for Redis pub/sub.
   - Example `statestore.yaml`:
     ```yaml
    apiVersion: dapr.io/v1alpha1
    kind: Component
    metadata:
      name: chat-state-store
    spec:
      type: state.redis
      version: v1
      metadata:
        - name: redisHost
          value: localhost:6379
        - name: redisPassword
          value: ""
        - name: actorStateStore
          value: "true"
        - name: keyPrefix
          value: none
     ```
   - Example `pubsub.yaml`:
     ```yaml
    apiVersion: dapr.io/v1alpha1
    kind: Component
    metadata:
      name: chat-pub-sub
    spec:
      type: pubsub.redis
      version: v1
      metadata:
        - name: redisHost
          value: localhost:6379
        - name: redisPassword
          value: ""
     ```
5. **Ensure Redis is available locally** for Dapr components.
6. **Update Aspire orchestration** to include Redis and Dapr sidecars as services.
7. **Document usage** in the solution's README.

## Notes

- All Aspire projects must reside in the `Aspire` folder.
- Dapr components must use Redis as the backend for both storage and pub/sub.
- The solution should be ready for local development and testing with Dapr and Redis.




