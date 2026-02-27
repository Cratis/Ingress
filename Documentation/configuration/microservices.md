# Microservices

Ingress routes requests to one or more **microservices** using [YARP](https://microsoft.github.io/reverse-proxy/).
Each microservice may expose a **backend** (API), a **frontend** (SPA / static assets), or both.

---

## Configuration

Microservices are configured under `Ingress:Microservices`, keyed by a friendly name:

```json
{
  "Ingress": {
    "Microservices": {
      "portal": {
        "Backend": { "BaseUrl": "http://portal-api:8080/" },
        "Frontend": { "BaseUrl": "http://portal-web:3000/" },
        "ResolveIdentityDetails": true
      },
      "catalog": {
        "Backend": { "BaseUrl": "http://catalog-api:8080/" }
      }
    }
  }
}
```

### MicroserviceConfig properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Backend` | `MicroserviceEndpointConfig` | `null` | API backend endpoint. |
| `Frontend` | `MicroserviceEndpointConfig` | `null` | SPA / static-asset frontend endpoint. |
| `ResolveIdentityDetails` | `bool?` | `true` when Backend is set | Whether to call `/.cratis/me` on this microservice to enrich the identity cookie. |

### MicroserviceEndpointConfig properties

| Property | Type | Description |
|----------|------|-------------|
| `BaseUrl` | `string` | Base URL of the endpoint (e.g. `http://my-service:8080/`). |

---

## Routing

### Single microservice

When only one microservice is configured, Ingress adds a plain catch-all route so the microservice
is reachable without any special routing header or query parameter.

- `/{**path}` → frontend
- `/api/{**path}` → backend

### Multiple microservices

With more than one microservice, clients must indicate the target using one of:

| Mechanism | Example |
|-----------|---------|
| `Microservice-ID` request header | `Microservice-ID: portal` |
| `microservice` query parameter | `?microservice=portal` |

Routes are matched case-insensitively.

---

## Identity enrichment

For each microservice with a `Backend` endpoint (and `ResolveIdentityDetails` not explicitly set to
`false`), Ingress calls `GET {Backend.BaseUrl}/.cratis/me` after authentication.
The response is stored in a short-lived HTTP-only cookie (`.cratis-identity`) and injected as
the `X-MS-CLIENT-PRINCIPAL` header on every proxied request so that backend services can read
identity details without re-calling the identity endpoint themselves.
