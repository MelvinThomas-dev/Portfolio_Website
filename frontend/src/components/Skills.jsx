import { skillsList } from '../data/resumeContent';
import ScrollReveal from './ScrollReveal';

export default function Skills() {
  return (
    <section id="skills" className="section section--alt">
      <div className="container">
        <ScrollReveal>
          <h2 className="section__title">Skills</h2>
        </ScrollReveal>
        <div className="skills__grid">
          {skillsList.map((group) => (
            <ScrollReveal key={group.category}>
              <div className="skill-card card-hover">
                <h3>{group.category}</h3>
                <ul>
                  {group.items.map((item) => (
                    <li key={item}>{item}</li>
                  ))}
                </ul>
              </div>
            </ScrollReveal>
          ))}
        </div>
      </div>
    </section>
  );
}
