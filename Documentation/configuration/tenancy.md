# Tenancy

Ingress resolves a **tenant ID** (GUID) from every incoming request and stores it in the request
context. Downstream microservices receive the resolved tenant ID via the `X-Tenant-ID` header.

---

## Tenant resolution strategies

Resolution strategies are evaluated **in order** until one succeeds.
Configure them under `Ingress:TenantResolutions`:

```json
{
  "Ingress": {
    "TenantResolutions": [
      { "Strategy": "Host" },
      { "Strategy": "Claim", "Options": { "ClaimType": "tid" } }
    ]
  }
}
```

### Available strategies

| Strategy | Description |
|----------|-------------|
| `Host` | Extracts the hostname from the `Host` header and looks it up in `Ingress:Tenants`. |
| `Claim` | Reads a claim value from the authenticated user's `ClaimsPrincipal`. |
| `Route` | Matches a regex pattern against the request path to extract a tenant identifier. |
| `Specified` | Uses a fixed, statically configured tenant ID for all requests. |

---

## Claim strategy options

```json
{
  "Strategy": "Claim",
  "Options": {
    "ClaimType": "tid"
  }
}
```

If `ClaimType` is omitted, the strategy falls back to reading the `X-MS-CLIENT-PRINCIPAL` header
(Azure App Service format).

---

## Route strategy options

```json
{
  "Strategy": "Route",
  "Options": {
    "Pattern": "^/tenant/(?<tenant>[^/]+)"
  }
}
```

The named group `tenant` is used as the tenant identifier, which is then matched against
`Ingress:Tenants`.

---

## Specified strategy options

```json
{
  "Strategy": "Specified",
  "Options": {
    "TenantId": "00000000-0000-0000-0000-000000000001"
  }
}
```

---

## Tenant registry

Each strategy (except `Specified`) resolves a **source identifier** string that is matched
against the `Ingress:Tenants` dictionary to obtain the final tenant GUID.

```json
{
  "Ingress": {
    "Tenants": {
      "00000000-0000-0000-0000-000000000001": {
        "SourceIdentifiers": [ "myapp.example.com" ]
      },
      "00000000-0000-0000-0000-000000000002": {
        "SourceIdentifiers": [ "otherapp.example.com" ]
      }
    }
  }
}
```

---

## Lobby fallback

When no tenant can be resolved and the [invite lobby](invites.md) is configured, the user is
redirected to the lobby frontend instead of receiving a `401 Unauthorized` response.
See [Invites & Lobby](invites.md) for details.

If no lobby is configured and `TenantResolutions` is non-empty, Ingress returns `401 Unauthorized`.
When `TenantResolutions` is empty (not configured), the request proceeds without a tenant.
