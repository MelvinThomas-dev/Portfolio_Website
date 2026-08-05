import { certifications } from '../data/resumeContent';
import ScrollReveal from './ScrollReveal';

export default function Certifications() {
  return (
    <section id="certifications" className="section section--alt">
      <div className="container">
        <ScrollReveal>
          <h2 className="section__title">Certifications</h2>
        </ScrollReveal>
        <div className="education__grid">
          {certifications.map((cert) => (
            <ScrollReveal key={cert.name}>
              <article className="education__item card-hover">
                <h4>{cert.name}</h4>
                <p>{cert.issuer}</p>
                {cert.year && <p className="muted">{cert.year}</p>}
              </article>
            </ScrollReveal>
          ))}
        </div>
      </div>
    </section>
  );
}
