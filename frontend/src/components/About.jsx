import { about } from '../data/resumeContent';
import ScrollReveal from './ScrollReveal';

export default function About() {
  return (
    <section id="about" className="section">
      <div className="container">
        <ScrollReveal>
          <h2 className="section__title">About Me</h2>
        </ScrollReveal>
        <ScrollReveal className="about__content">
          {about.summary.map((paragraph) => (
            <p key={paragraph.slice(0, 40)}>{paragraph}</p>
          ))}
        </ScrollReveal>
      </div>
    </section>
  );
}
