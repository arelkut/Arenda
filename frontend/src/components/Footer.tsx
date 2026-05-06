import { Home, Mail, Phone, MapPin } from 'lucide-react';
import { Link } from 'react-router-dom';
import styles from './Footer.module.css';

export default function Footer() {
  return (
    <footer className={styles.footer}>
      <div className={`container ${styles.inner}`}>
        <div className={styles.grid}>
          <div className={styles.col}>
            <Link to="/" className={styles.logo}>
              <Home size={24} />
              <span>Недвижимость.РФ</span>
            </Link>
            <p className={styles.desc}>
              Платформа для поиска и аренды недвижимости по всей России
            </p>
          </div>
          <div className={styles.col}>
            <h4 className={styles.colTitle}>Навигация</h4>
            <Link to="/" className={styles.link}>Главная</Link>
            <Link to="/catalog" className={styles.link}>Каталог</Link>
            <Link to="/mortgage" className={styles.link}>Ипотека</Link>
            <Link to="/blog" className={styles.link}>Блог</Link>
            <Link to="/analytics" className={styles.link}>Аналитика</Link>
          </div>
          <div className={styles.col}>
            <h4 className={styles.colTitle}>Услуги</h4>
            <Link to="/catalog" className={styles.link}>Аренда квартир</Link>
            <Link to="/catalog" className={styles.link}>Аренда домов</Link>
            <Link to="/catalog" className={styles.link}>Коммерческая</Link>
            <Link to="/reviews" className={styles.link}>Отзывы</Link>
          </div>
          <div className={styles.col}>
            <h4 className={styles.colTitle}>Контакты</h4>
            <div className={styles.contact}><Phone size={16} /> +7 (800) 123-45-67</div>
            <div className={styles.contact}><Mail size={16} /> info@nedvizhimost.rf</div>
            <div className={styles.contact}><MapPin size={16} /> Москва, ул. Примерная, 1</div>
          </div>
        </div>
        <div className={styles.bottom}>
          <span>&copy; 2024 Недвижимость.РФ. Все права защищены.</span>
        </div>
      </div>
    </footer>
  );
}
