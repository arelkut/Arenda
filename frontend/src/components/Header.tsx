import { Link, useLocation } from 'react-router-dom';
import { Home, LogIn, LogOut, User } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import styles from './Header.module.css';

export default function Header() {
  const location = useLocation();
  const { isAuthenticated, email, logout } = useAuth();

  const navItems = [
    { path: '/', label: 'Главная' },
    { path: '/catalog', label: 'Каталог' },
    { path: '/mortgage', label: 'Ипотека' },
    { path: '/blog', label: 'Блог' },
    { path: '/analytics', label: 'Аналитика' },
  ];

  return (
    <header className={styles.header}>
      <div className={`container ${styles.inner}`}>
        <Link to="/" className={styles.logo}>
          <Home size={28} />
          <span>Недвижимость.РФ</span>
        </Link>
        <nav className={styles.nav}>
          {navItems.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              className={`${styles.navLink} ${location.pathname === item.path ? styles.active : ''}`}
            >
              {item.label}
            </Link>
          ))}
        </nav>
        <div className={styles.auth}>
          {isAuthenticated ? (
            <div className={styles.userMenu}>
              <span className={styles.userEmail}><User size={16} /> {email}</span>
              <button className={`btn btn-outline ${styles.logoutBtn}`} onClick={logout}>
                <LogOut size={16} /> Выйти
              </button>
            </div>
          ) : (
            <Link to="/login" className={`btn btn-primary ${styles.loginBtn}`}>
              <LogIn size={16} /> Войти
            </Link>
          )}
        </div>
      </div>
    </header>
  );
}
