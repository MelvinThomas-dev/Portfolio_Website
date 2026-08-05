import { lazy, Suspense } from 'react';
import { useMediaQuery } from '../hooks/useMediaQuery';

const HeroScene3D = lazy(() => import('./HeroScene3D'));

export default function HeroBackground3D() {
  const prefersReducedMotion = useMediaQuery('(prefers-reduced-motion: reduce)');
  const isDesktop = useMediaQuery('(min-width: 769px)');

  if (prefersReducedMotion || !isDesktop) return null;

  return (
    <div className="hero__scene" aria-hidden="true">
      <Suspense fallback={null}>
        <HeroScene3D />
      </Suspense>
    </div>
  );
}
