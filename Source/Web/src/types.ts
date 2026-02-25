export type OidcProviderType = 'Custom' | 'Microsoft' | 'Google' | 'GitHub' | 'Apple';

export interface OidcProvider {
    name: string;
    type: OidcProviderType;
    loginUrl: string;
}
