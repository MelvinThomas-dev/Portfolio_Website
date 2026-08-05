import { education } from '../data/resumeContent';
import ScrollReveal from './ScrollReveal';

export default function Education() {
  return (
    <section id="education" className="section">
      <div className="container">
        <ScrollReveal>
          <h2 className="section__title">Education</h2>
        </ScrollReveal>
        <div className="education__grid">
          {education.map((edu) => (
            <ScrollReveal key={edu.institution}>
              <article className="education__item card-hover">
                <h4>{edu.degree}</h4>
                <p>{edu.institution}</p>
                <p className="muted">
                  {edu.period} · {edu.location} · {edu.details}
                </p>
              </article>
            </ScrollReveal>
          ))}
        </div>
      </div>
    </section>
  );
}
