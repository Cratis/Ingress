# Configuration

Cratis Ingress is configured entirely through the `Ingress` section of `appsettings.json`
(or equivalent environment variables using the `Ingress__` prefix).

```json
{
  "Ingress": {
    "OidcProviders": [ ... ],
    "TenantResolutions": [ ... ],
    "Tenants": { ... },
    "Microservices": { ... },
    "Invite": { ... }
  }
}
```

| Topic | Description |
|-------|-------------|
| [Authentication](authentication.md) | OIDC providers and JWT Bearer configuration. |
| [Tenancy](tenancy.md) | How the ingress resolves the current tenant from each request. |
| [Microservices](microservices.md) | Routing requests to backend and frontend microservices. |
| [Invites & Lobby](invites.md) | Invite-based onboarding and the lobby microservice. |
