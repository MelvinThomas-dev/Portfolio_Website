import { experience } from '../data/resumeContent';
import ScrollReveal from './ScrollReveal';

export default function Experience() {
  return (
    <section id="experience" className="section">
      <div className="container">
        <ScrollReveal>
          <h2 className="section__title">Experience</h2>
        </ScrollReveal>
        <div className="timeline">
          {experience.map((job) => (
            <ScrollReveal key={`${job.company}-${job.role}`}>
              <article className="timeline__item card-hover">
                <div className="timeline__header">
                  <div>
                    <h3>{job.role}</h3>
                    <p className="timeline__company">{job.company}</p>
                  </div>
                  <div className="timeline__meta">
                    <span>{job.period}</span>
                    <span>{job.location}</span>
                  </div>
                </div>
                <ul>
                  {job.highlights.map((item) => (
                    <li key={item.slice(0, 50)}>{item}</li>
                  ))}
                </ul>
              </article>
            </ScrollReveal>
          ))}
        </div>
      </div>
    </section>
  );
}
