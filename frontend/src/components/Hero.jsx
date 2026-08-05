import { profile } from '../data/resumeContent';
import ScrollReveal from './ScrollReveal';
import HeroBackground3D from './HeroBackground3D';

export default function Hero() {
  const scrollToContact = () => {
    document.getElementById('contact')?.scrollIntoView({ behavior: 'smooth' });
  };

  return (
    <section id="hero" className="hero section">
      <HeroBackground3D />
      <div className="container hero__grid">
        <ScrollReveal className="hero__content">
          <p className="hero__greeting">Hello, I&apos;m</p>
          <h1 className="hero__name">{profile.name}</h1>
          <h2 className="hero__title">{profile.title}</h2>
          <p className="hero__location">{profile.location}</p>
          <p className="hero__pitch">{profile.pitch}</p>
          <div className="hero__actions">
            <button type="button" className="btn btn--primary" onClick={scrollToContact}>
              Get in Touch
            </button>
            <a href={profile.linkedin} target="_blank" rel="noopener noreferrer" className="btn btn--outline">
              LinkedIn
            </a>
            <a href={profile.github} target="_blank" rel="noopener noreferrer" className="btn btn--outline">
              GitHub
            </a>
          </div>
        </ScrollReveal>
        <ScrollReveal className="hero__avatar">
          <div className="hero__avatar-circle">
            <span>MT</span>
          </div>
        </ScrollReveal>
      </div>
    </section>
  );
}
