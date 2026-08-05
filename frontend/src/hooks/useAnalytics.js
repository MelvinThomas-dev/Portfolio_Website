import { useEffect, useRef } from 'react';
import { useLocation } from 'react-router-dom';
import { recordVisit } from '../services/api';
import { getDeviceType } from './useDeviceInfo';
import { getSessionId } from '../utils/visitor';

const VISIT_DEDUP_WINDOW_MS = 30 * 60 * 1000;
const inFlightVisits = new Set();

function getVisitStorageKey(sessionId, pagePath) {
  return `analytics_visit:${sessionId}:${pagePath}`;
}

function shouldRecordVisit(sessionId, pagePath) {
  const key = getVisitStorageKey(sessionId, pagePath);
  const lastRecorded = localStorage.getItem(key);
  if (lastRecorded) {
    const elapsed = Date.now() - Number(lastRecorded);
    if (elapsed < VISIT_DEDUP_WINDOW_MS) return false;
  }
  return true;
}

export function useAnalytics() {
  const location = useLocation();
  const lastPathRef = useRef(null);

  useEffect(() => {
    const pagePath = location.pathname + location.search;
    if (lastPathRef.current === pagePath) return;
    lastPathRef.current = pagePath;

    const sessionId = getSessionId();
    const dedupKey = `${sessionId}:${pagePath}`;

    if (inFlightVisits.has(dedupKey)) return;
    inFlightVisits.add(dedupKey);

    if (!shouldRecordVisit(sessionId, pagePath)) {
      inFlightVisits.delete(dedupKey);
      return;
    }

    const deviceType = getDeviceType();
    recordVisit(pagePath, deviceType)
      .then(() => {
        localStorage.setItem(getVisitStorageKey(sessionId, pagePath), String(Date.now()));
      })
      .catch(() => {
        localStorage.removeItem(getVisitStorageKey(sessionId, pagePath));
      })
      .finally(() => {
        setTimeout(() => inFlightVisits.delete(dedupKey), 1000);
      });
  }, [location.pathname, location.search]);
}
