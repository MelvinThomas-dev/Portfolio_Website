const SESSION_KEY = 'portfolio_session_id';
const LANDING_KEY = 'portfolio_landing_page';

function generateId() {
  if (typeof crypto !== 'undefined' && crypto.randomUUID) {
    return crypto.randomUUID();
  }
  return `${Date.now()}-${Math.random().toString(36).slice(2, 11)}`;
}

export function getSessionId() {
  let id = localStorage.getItem(SESSION_KEY);
  if (!id) {
    id = generateId();
    localStorage.setItem(SESSION_KEY, id);
  }
  return id;
}

export function getLandingPage() {
  let landing = sessionStorage.getItem(LANDING_KEY);
  if (!landing) {
    landing = `${window.location.pathname}${window.location.search}`;
    sessionStorage.setItem(LANDING_KEY, landing);
  }
  return landing;
}

export function getUtmParams() {
  const params = new URLSearchParams(window.location.search);
  return {
    utmSource: params.get('utm_source') || '',
    utmMedium: params.get('utm_medium') || '',
    utmCampaign: params.get('utm_campaign') || '',
  };
}

export function getVisitorMetadata() {
  const { utmSource, utmMedium, utmCampaign } = getUtmParams();
  return {
    referrer: document.referrer || '',
    landingPage: getLandingPage(),
    utmSource,
    utmMedium,
    utmCampaign,
    sessionId: getSessionId(),
  };
}
