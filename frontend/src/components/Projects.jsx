import { projects } from '../data/resumeContent';
import ScrollReveal from './ScrollReveal';

export default function Projects() {
  return (
    <section id="projects" className="section section--alt">
      <div className="container">
        <ScrollReveal>
          <h2 className="section__title">Featured Projects</h2>
        </ScrollReveal>
        <div className="projects__grid">
          {projects.map((project) => (
            <ScrollReveal key={project.title}>
              <article className="project-card card-hover">
                <h3>{project.title}</h3>
                <p>{project.description}</p>
                <div className="project-card__tech">
                  {project.tech.map((tech) => (
                    <span key={tech} className="tag">
                      {tech}
                    </span>
                  ))}
                </div>
                <ul>
                  {project.highlights.map((highlight) => (
                    <li key={highlight.slice(0, 50)}>{highlight}</li>
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
