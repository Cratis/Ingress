import { Button } from 'primereact/button';
import type { OidcProvider } from '../types';

interface ProviderButtonProps {
    provider: OidcProvider;
    returnUrl: string;
}

const providerIconClass: Record<string, string> = {
    Microsoft: 'pi pi-microsoft',
    Google: 'pi pi-google',
    GitHub: 'pi pi-github',
    Apple: 'pi pi-apple',
    Custom: 'pi pi-sign-in',
};

export const ProviderButton = ({ provider, returnUrl }: ProviderButtonProps) => {
    const iconClass = providerIconClass[provider.type] ?? providerIconClass.Custom;
    const loginUrl = returnUrl
        ? `${provider.loginUrl}?returnUrl=${encodeURIComponent(returnUrl)}`
        : provider.loginUrl;

    return (
        <a href={loginUrl} className="block w-full">
            <Button
                label={`Sign in with ${provider.name}`}
                icon={iconClass}
                className="w-full p-button-outlined"
            />
        </a>
    );
};
