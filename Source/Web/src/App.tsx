import { useEffect, useState } from 'react';
import { Card } from 'primereact/card';
import { ProgressSpinner } from 'primereact/progressspinner';
import { ProviderButton } from './components/ProviderButton';
import type { OidcProvider } from './types';

export default function App() {
    const [providers, setProviders] = useState<OidcProvider[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const returnUrl = new URLSearchParams(window.location.search).get('returnUrl') ?? '/';

    useEffect(() => {
        fetch('/.cratis/providers')
            .then(r => {
                if (!r.ok) throw new Error(`HTTP ${r.status}`);
                return r.json() as Promise<OidcProvider[]>;
            })
            .then(data => {
                setProviders(data);
                setLoading(false);
            })
            .catch(err => {
                setError(String(err));
                setLoading(false);
            });
    }, []);

    return (
        <div className="flex min-h-screen items-center justify-center">
            <Card
                title="Sign In"
                className="w-full max-w-sm shadow-lg"
            >
                {loading && (
                    <div className="flex justify-center py-4">
                        <ProgressSpinner />
                    </div>
                )}
                {error && (
                    <p className="text-red-400 text-sm text-center">{error}</p>
                )}
                {!loading && !error && providers.length === 0 && (
                    <p className="text-center text-sm opacity-70">No providers configured.</p>
                )}
                {!loading && !error && providers.length > 0 && (
                    <div className="flex flex-col gap-3">
                        {providers.map(p => (
                            <ProviderButton key={p.name} provider={p} returnUrl={returnUrl} />
                        ))}
                    </div>
                )}
            </Card>
        </div>
    );
}
