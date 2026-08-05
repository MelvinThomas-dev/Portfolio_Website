import { useEffect, useState } from 'react';
import { navLinks, profile } from '../data/resumeContent';
import { useTheme } from '../hooks/useTheme';

export default function Navbar() {
  const [scrolled, setScrolled] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const { theme, toggleTheme } = useTheme();

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 20);
    window.addEventListener('scroll', onScroll);
    return () => window.removeEventListener('scroll', onScroll);
  }, []);

  const scrollTo = (id) => {
    document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
    setMenuOpen(false);
  };

  return (
    <header className={`navbar ${scrolled ? 'navbar--scrolled' : ''}`}>
      <div className="container navbar__inner">
        <button type="button" className="navbar__brand" onClick={() => scrollTo('hero')}>
          {profile.name.split(' ')[0]}
          <span className="accent">.</span>
        </button>

        <button
          type="button"
          className="navbar__toggle"
          aria-label="Toggle menu"
          onClick={() => setMenuOpen((open) => !open)}
        >
          <span />
          <span />
          <span />
        </button>

        <nav className={`navbar__links ${menuOpen ? 'navbar__links--open' : ''}`}>
          {navLinks.map((link) => (
            <button key={link.id} type="button" className="nav-link" onClick={() => scrollTo(link.id)}>
              {link.label}
            </button>
          ))}
          <button
            type="button"
            className="theme-toggle"
            onClick={toggleTheme}
            aria-label={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
          >
            {theme === 'dark' ? '☀️' : '🌙'}
          </button>
          <a href={profile.resumeUrl} className="btn btn--outline btn--sm" download>
            Resume
          </a>
        </nav>
      </div>
    </header>
  );
}
