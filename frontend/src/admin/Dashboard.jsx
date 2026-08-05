import { Fragment, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Bar,
  BarChart,
  Cell,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import { fetchDashboard } from '../services/api';
import { useTheme } from '../hooks/useTheme';

const CHART_COLORS = ['#0d6e8a', '#1a9bb8', '#2ec4d6', '#5dd4e3', '#94e2ed', '#c5f0f6'];

function StatCard({ label, total, unique }) {
  return (
    <div className="stat-card card-hover">
      <h3>{label}</h3>
      <p className="stat-card__value">{total}</p>
      <p className="muted">{unique} unique sessions</p>
    </div>
  );
}

function LocationTable({ title, headers, rows, emptyMessage }) {
  return (
    <div className="location-table card-hover">
      <h3>{title}</h3>
      {rows.length === 0 ? (
        <p className="muted">{emptyMessage}</p>
      ) : (
        <div className="location-table__scroll">
          <table>
            <thead>
              <tr>
                {headers.map((header) => (
                  <th key={header}>{header}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {rows.map((row) => (
                <tr key={row.key}>
                  {row.cells.map((cell) => (
                    <td key={cell}>{cell}</td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default function Dashboard() {
  const navigate = useNavigate();
  const { theme, toggleTheme } = useTheme();
  const [stats, setStats] = useState(null);
  const [error, setError] = useState('');
  const [expandedId, setExpandedId] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem('admin_token');
    const expires = localStorage.getItem('admin_token_expires');

    if (!token || (expires && new Date(expires) < new Date())) {
      navigate('/admin');
      return;
    }

    fetchDashboard(token)
      .then(setStats)
      .catch((err) => {
        if (err.message.includes('401') || err.message.toLowerCase().includes('unauthorized')) {
          localStorage.removeItem('admin_token');
          navigate('/admin');
        } else {
          setError(err.message);
        }
      });
  }, [navigate]);

  const logout = () => {
    localStorage.removeItem('admin_token');
    localStorage.removeItem('admin_token_expires');
    navigate('/admin');
  };

  if (error) {
    return (
      <div className="admin-page">
        <p className="form-status form-status--error">{error}</p>
      </div>
    );
  }

  if (!stats) {
    return (
      <div className="admin-page">
        <p>Loading dashboard…</p>
      </div>
    );
  }

  const countryRows = stats.countries.map((entry) => ({
    key: entry.label,
    cells: [entry.label, entry.count],
  }));

  const cityRows = stats.cities.map((entry) => ({
    key: `${entry.city}-${entry.country}`,
    cells: [entry.city, entry.country || 'Unknown', entry.count],
  }));

  return (
    <div className="admin-page">
      <header className="admin-header">
        <h1>Analytics Dashboard</h1>
        <div className="admin-header__actions">
          <button type="button" className="theme-toggle" onClick={toggleTheme} aria-label="Toggle theme">
            {theme === 'dark' ? '☀️' : '🌙'}
          </button>
          <button type="button" className="btn btn--outline btn--sm" onClick={() => window.location.reload()}>
            Refresh
          </button>
          <button type="button" className="btn btn--outline btn--sm" onClick={logout}>
            Logout
          </button>
        </div>
      </header>

      <div className="stats-grid">
        <StatCard label="All Time" total={stats.visits.totalAllTime} unique={stats.visits.uniqueSessionsAllTime} />
        <StatCard label="Last 7 Days" total={stats.visits.total7Days} unique={stats.visits.uniqueSessions7Days} />
        <StatCard label="Last 30 Days" total={stats.visits.total30Days} unique={stats.visits.uniqueSessions30Days} />
      </div>

      <div className="charts-grid">
        <div className="chart-card card-hover">
          <h3>Top Countries</h3>
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={stats.topCountries}>
              <XAxis dataKey="label" tick={{ fontSize: 12 }} />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Bar dataKey="count" fill="#0d6e8a" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div className="chart-card card-hover">
          <h3>Devices</h3>
          <ResponsiveContainer width="100%" height={260}>
            <PieChart>
              <Pie data={stats.devices} dataKey="count" nameKey="label" cx="50%" cy="50%" outerRadius={90} label>
                {stats.devices.map((entry, index) => (
                  <Cell key={entry.label} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div className="chart-card card-hover">
          <h3>Traffic Sources</h3>
          <ResponsiveContainer width="100%" height={260}>
            <PieChart>
              <Pie
                data={stats.trafficSources}
                dataKey="count"
                nameKey="label"
                cx="50%"
                cy="50%"
                outerRadius={90}
                label
              >
                {stats.trafficSources.map((entry, index) => (
                  <Cell key={entry.label} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div className="chart-card card-hover">
          <h3>Top Cities</h3>
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={stats.topCities} layout="vertical">
              <XAxis type="number" allowDecimals={false} />
              <YAxis type="category" dataKey="label" width={100} tick={{ fontSize: 12 }} />
              <Tooltip />
              <Bar dataKey="count" fill="#1a9bb8" radius={[0, 4, 4, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="location-tables">
        <LocationTable
          title={`All Countries (${stats.countries.length})`}
          headers={['Country', 'Visits']}
          rows={countryRows}
          emptyMessage="No country data recorded yet."
        />
        <LocationTable
          title={`All Cities (${stats.cities.length})`}
          headers={['City', 'Country', 'Visits']}
          rows={cityRows}
          emptyMessage="No city data recorded yet."
        />
      </div>

      <div className="submissions-table card-hover">
        <h3>Contact Submissions</h3>
        {stats.submissions.length === 0 ? (
          <p className="muted">No submissions yet.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Subject</th>
                <th>Location</th>
                <th>Device</th>
                <th>Source</th>
                <th>Date</th>
              </tr>
            </thead>
            <tbody>
              {stats.submissions.map((submission) => (
                <Fragment key={submission.id}>
                  <tr
                    className="submissions-table__row"
                    onClick={() => setExpandedId(expandedId === submission.id ? null : submission.id)}
                  >
                    <td>{submission.name}</td>
                    <td>{submission.email}</td>
                    <td>{submission.subject}</td>
                    <td>{submission.location}</td>
                    <td>{submission.deviceType}</td>
                    <td>{submission.trafficSource}</td>
                    <td>{new Date(submission.createdAt).toLocaleString()}</td>
                  </tr>
                  {expandedId === submission.id && (
                    <tr className="submissions-table__expanded">
                      <td colSpan={7}>
                        <strong>Message:</strong> {submission.message}
                      </td>
                    </tr>
                  )}
                </Fragment>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
