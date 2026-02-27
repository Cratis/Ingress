# Authentication

Ingress supports two authentication modes that can be active simultaneously:

- **Interactive browser sessions** – OpenID Connect (OIDC) with a cookie.
- **Machine-to-machine / API** – JWT Bearer tokens.

---

## Single OIDC provider

Bind a single OpenID Connect provider using the standard `Authentication:OpenIdConnect` section:

```json
{
  "Authentication": {
    "OpenIdConnect": {
      "Authority": "https://login.microsoftonline.com/<tenant-id>/v2.0",
      "ClientId": "<client-id>",
      "ClientSecret": "<client-secret>"
    }
  }
}
```

---

## Multiple OIDC providers

When users can choose from more than one identity provider, list them under `Ingress:OidcProviders`.
Ingress will redirect unauthenticated browser requests to a built-in provider-selection page
(`/.cratis/select-provider`) instead of challenging with a single provider directly.

```json
{
  "Ingress": {
    "OidcProviders": [
      {
        "Name": "Microsoft",
        "Type": "Microsoft",
        "Authority": "https://login.microsoftonline.com/<tenant-id>/v2.0",
        "ClientId": "<client-id>",
        "ClientSecret": "<client-secret>",
        "Scopes": []
      },
      {
        "Name": "Google",
        "Type": "Google",
        "Authority": "https://accounts.google.com",
        "ClientId": "<client-id>",
        "ClientSecret": "<client-secret>",
        "Scopes": []
      }
    ]
  }
}
```

Each provider generates a dedicated login endpoint at `/.cratis/login/{scheme}`.
The scheme name is derived from the provider `Name` by lowercasing and replacing spaces with hyphens
(e.g. `"My Provider"` → `/.cratis/login/my-provider`).

### OidcProviderConfig properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Display name shown on the login selection page. |
| `Type` | `string` | Provider type hint (`Microsoft`, `Google`, or `Custom`). |
| `Authority` | `string` | OIDC authority URL. |
| `ClientId` | `string` | OAuth 2.0 client ID. |
| `ClientSecret` | `string` | OAuth 2.0 client secret. |
| `Scopes` | `string[]` | Additional scopes to request (beyond `openid`, `profile`, `email`). |

---

## JWT Bearer (API)

For machine-to-machine calls, configure a JWT Bearer handler:

```json
{
  "Authentication": {
    "JwtBearer": {
      "Authority": "https://login.microsoftonline.com/<tenant-id>/v2.0",
      "Audience": "<api-audience>"
    }
  }
}
```
