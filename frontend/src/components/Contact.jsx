import { useState } from 'react';
import { submitContact } from '../services/api';
import { getDeviceType } from '../hooks/useDeviceInfo';
import ScrollReveal from './ScrollReveal';
import { SocialIconGroup } from './SocialIcons';

const CONTACT_SUBMITTED_KEY = 'contactFormSubmitted';
const initialForm = { name: '', email: '', subject: '', message: '' };

export default function Contact() {
  const [form, setForm] = useState(initialForm);
  const [status, setStatus] = useState({ type: '', message: '' });
  const [submitting, setSubmitting] = useState(false);
  const [submitted, setSubmitted] = useState(
    () => localStorage.getItem(CONTACT_SUBMITTED_KEY) === 'true',
  );

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setStatus({ type: '', message: '' });

    try {
      await submitContact(form, getDeviceType());
      localStorage.setItem(CONTACT_SUBMITTED_KEY, 'true');
      setSubmitted(true);
      setForm(initialForm);
    } catch (err) {
      setStatus({ type: 'error', message: err.message || 'Failed to send message. Please try again.' });
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <section id="contact" className="section section--alt">
      <div className="container">
        <ScrollReveal>
          <h2 className="section__title">Contact</h2>
        </ScrollReveal>
        <div className="contact__grid">
          <ScrollReveal className="contact__info">
            <p>Interested in working together or have a question? Send a message or reach out directly.</p>
            <SocialIconGroup className="contact__social" />
          </ScrollReveal>

          {submitted ? (
            <ScrollReveal>
              <div className="contact__success card-hover">
                <h3>Thank you!</h3>
                <p>Your message has been sent. I&apos;ll get back to you as soon as possible.</p>
              </div>
            </ScrollReveal>
          ) : (
            <ScrollReveal>
              <form className="contact__form card-hover" onSubmit={handleSubmit}>
                <div className="form-row">
                  <label htmlFor="name">Name</label>
                  <input id="name" name="name" value={form.name} onChange={handleChange} required maxLength={100} />
                </div>
                <div className="form-row">
                  <label htmlFor="email">Email</label>
                  <input
                    id="email"
                    name="email"
                    type="email"
                    value={form.email}
                    onChange={handleChange}
                    required
                    maxLength={200}
                  />
                </div>
                <div className="form-row">
                  <label htmlFor="subject">Subject</label>
                  <input
                    id="subject"
                    name="subject"
                    value={form.subject}
                    onChange={handleChange}
                    required
                    maxLength={200}
                  />
                </div>
                <div className="form-row">
                  <label htmlFor="message">Message</label>
                  <textarea
                    id="message"
                    name="message"
                    rows={5}
                    value={form.message}
                    onChange={handleChange}
                    required
                    maxLength={5000}
                  />
                </div>
                {status.message && <p className={`form-status form-status--${status.type}`}>{status.message}</p>}
                <button type="submit" className="btn btn--primary" disabled={submitting}>
                  {submitting ? 'Sending…' : 'Send Message'}
                </button>
              </form>
            </ScrollReveal>
          )}
        </div>
      </div>
    </section>
  );
}
