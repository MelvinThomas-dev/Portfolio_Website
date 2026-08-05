import { personal } from '../data/resumeContent';
import { SocialIconGroup } from './SocialIcons';

export default function Footer() {
  const year = new Date().getFullYear();

  return (
    <footer className="footer">
      <div className="container footer__inner">
        <div className="footer__top">
          <p className="footer__brand">{personal.name}</p>
          <SocialIconGroup className="footer__social" />
        </div>
        <p className="footer__privacy">
          Anonymous usage analytics collected to improve this site. No personal data is tracked from casual
          visitors beyond an anonymous session identifier stored locally.
        </p>
        <p className="footer__copy">&copy; {year} {personal.name}. All rights reserved.</p>
      </div>
    </footer>
  );
}
