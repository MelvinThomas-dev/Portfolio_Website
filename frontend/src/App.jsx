import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { useAnalytics } from './hooks/useAnalytics';
import Navbar from './components/Navbar';
import Hero from './components/Hero';
import About from './components/About';
import Skills from './components/Skills';
import Experience from './components/Experience';
import Projects from './components/Projects';
import Education from './components/Education';
import Certifications from './components/Certifications';
import Contact from './components/Contact';
import Footer from './components/Footer';
import AdminLogin from './admin/Login';
import Dashboard from './admin/Dashboard';

function PortfolioPage() {
  useAnalytics();

  return (
    <>
      <Navbar />
      <main>
        <Hero />
        <About />
        <Skills />
        <Experience />
        <Projects />
        <Education />
        <Certifications />
        <Contact />
      </main>
      <Footer />
    </>
  );
}

function App() {
  return (
    <BrowserRouter basename="/Portfolio_Website">
      <Routes>
        <Route path="/" element={<PortfolioPage />} />
        <Route path="/admin" element={<AdminLogin />} />
        <Route path="/admin/dashboard" element={<Dashboard />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
