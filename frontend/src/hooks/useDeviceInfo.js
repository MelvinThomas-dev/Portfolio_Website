export function getDeviceType() {
  const ua = navigator.userAgent.toLowerCase();
  const isTablet =
    /ipad|tablet|playbook|silk/.test(ua) ||
    (ua.includes('android') && !ua.includes('mobile'));

  if (isTablet) return 'Tablet';

  const isMobile = /iphone|ipod|android.*mobile|windows phone|blackberry|mobile/.test(ua);
  if (isMobile) return 'Mobile';

  if (window.innerWidth < 768) return 'Mobile';
  if (window.innerWidth < 1024) return 'Tablet';

  return 'Desktop';
}
