export const personal = {
  name: 'Melvin Thomas D',
  title: '.NET Full Stack Developer',
  location: 'Chennai, India',
  tagline:
    'Building enterprise healthcare applications with ASP.NET Core, Angular & React, SQL Server, and Azure DevOps.',
  email: 'melthomas220@gmail.com',
  linkedin: 'https://linkedin.com/in/melvin-thomas-developer',
  github: 'https://github.com/MelvinThomas-dev',
  phone: '',
  resumeUrl: '/Portfolio_Website/Melvin_Thomas_D.pdf',
};

export const profile = {
  ...personal,
  pitch: personal.tagline,
};

export const about = {
  summary: [
    'Software developer with ~2 years of experience building enterprise healthcare applications at Grid Dynamics. I specialize in full-stack development using ASP.NET Core, Angular, React, and SQL Server, delivering scalable solutions that improve clinical workflows and patient outcomes.',
    'Passionate about clean architecture, testable code, and CI/CD practices. I enjoy bridging frontend UX with robust backend APIs and have hands-on experience with Azure DevOps pipelines, cloud deployments, and AI-assisted development workflows.',
    'Currently seeking opportunities to grow as a .NET Full Stack Developer where I can contribute to meaningful products and continue expanding my expertise in cloud-native architectures.',
  ],
};

export const skills = {
  Frontend: ['Angular', 'React', 'TypeScript', 'JavaScript', 'HTML5', 'CSS3', 'Bootstrap', 'Responsive UI'],
  Backend: ['ASP.NET Core', 'C#', '.NET 6/8', 'RESTful APIs', 'Entity Framework Core', 'LINQ'],
  Database: ['SQL Server', 'PostgreSQL', 'T-SQL', 'Database Design', 'Stored Procedures'],
  DevOps: ['Azure DevOps', 'Git', 'CI/CD Pipelines', 'Docker', 'Azure App Service'],
  'AI / Automation': ['Azure OpenAI', 'Prompt Engineering', 'AI-Assisted Workflows', 'Requirement Automation'],
  Practices: ['Agile/Scrum', 'Code Reviews', 'Unit Testing', 'Clean Architecture', 'SOLID Principles'],
};

export const skillsList = Object.entries(skills).map(([category, items]) => ({ category, items }));

export const experience = [
  {
    company: 'Grid Dynamics',
    role: 'Junior Software Developer',
    period: '2024 – Present',
    location: 'Chennai, India',
    highlights: [
      'Develop and maintain enterprise healthcare web applications using ASP.NET Core and Angular for clinical and operational teams.',
      'Design RESTful APIs and integrate with SQL Server databases, optimizing queries and data access patterns for performance.',
      'Collaborate in Agile sprints with cross-functional teams; participate in code reviews, testing, and Azure DevOps CI/CD pipelines.',
      'Implement role-based authentication, CRUD modules, and dashboard features aligned with healthcare compliance requirements.',
    ],
  },
  {
    company: 'Grid Dynamics',
    role: 'UI Intern',
    period: '2023 – 2024',
    location: 'Chennai, India',
    highlights: [
      'Built responsive Angular components and pages for internal healthcare management tools.',
      'Worked closely with senior developers to translate wireframes into production-ready UI with consistent design patterns.',
      'Gained foundational experience in TypeScript, RxJS, and component-based architecture.',
    ],
  },
];

export const projects = [
  {
    title: 'Enterprise Management System',
    description:
      'Full-stack enterprise application with role-based authentication, CRUD operations, and interactive dashboards for managing organizational data and workflows.',
    tech: ['Angular', 'ASP.NET Core', 'SQL Server', 'Entity Framework Core'],
    highlights: [
      'Implemented JWT-based role-based access control across modules.',
      'Built reusable Angular components and REST API endpoints with EF Core.',
      'Designed SQL Server schema with optimized queries for dashboard analytics.',
    ],
  },
  {
    title: 'AI Requirement Assistant',
    description:
      'Intelligent tool that transforms business requirements into structured user stories and development tasks using Azure OpenAI, accelerating sprint planning.',
    tech: ['Angular', 'ASP.NET Core', 'Azure OpenAI', 'REST APIs'],
    highlights: [
      'Integrated Azure OpenAI API for natural language requirement parsing.',
      'Designed prompt templates to generate consistent user stories and task breakdowns.',
      'Built an Angular UI for input, review, and export of generated artifacts.',
    ],
  },
];

export const education = [
  {
    institution: 'Jeppiaar Institute of Technology',
    degree: 'B.E. Computer Science & Engineering',
    period: '2020 – 2024',
    location: 'Chennai',
    details: 'CGPA: 8.7 / 10',
  },
];

export const certifications = [
  {
    name: 'Google Cloud Certified Specialist – Foundational Level',
    issuer: 'Google Cloud',
    year: '2024',
  },
];

export const navLinks = [
  { id: 'hero', label: 'Home' },
  { id: 'about', label: 'About' },
  { id: 'skills', label: 'Skills' },
  { id: 'experience', label: 'Experience' },
  { id: 'projects', label: 'Projects' },
  { id: 'education', label: 'Education' },
  { id: 'certifications', label: 'Certifications' },
  { id: 'contact', label: 'Contact' },
];
