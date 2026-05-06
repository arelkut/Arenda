import { useState } from 'react';
import { Link } from 'react-router-dom';
import { KeyRound, Home, ArrowLeft } from 'lucide-react';
import styles from './Auth.module.css';

export default function ForgotPassword() {
  const [email, setEmail] = useState('');
  const [sent, setSent] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSent(true);
  };

  return (
    <div className={styles.page}>
      <div className={styles.card}>
        <Link to="/" className={styles.logo}>
          <Home size={28} />
          <span>Недвижимость.РФ</span>
        </Link>
        <h2 className={styles.title}>Восстановление пароля</h2>
        <p className={styles.subtitle}>Введите email для получения инструкций</p>

        {sent ? (
          <div className={styles.successMsg}>
            <KeyRound size={48} />
            <h3>Письмо отправлено!</h3>
            <p>Проверьте почту {email}. Мы отправили инструкции по восстановлению пароля.</p>
            <Link to="/login" className={`btn btn-primary btn-lg ${styles.submitBtn}`}>
              <ArrowLeft size={18} /> Вернуться к входу
            </Link>
          </div>
        ) : (
          <form onSubmit={handleSubmit}>
            <div className={styles.formGroup}>
              <label className="input-label">Email</label>
              <input
                className="input-field"
                type="email"
                placeholder="your@email.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <button type="submit" className={`btn btn-primary btn-lg ${styles.submitBtn}`}>
              <KeyRound size={18} /> Отправить инструкции
            </button>
          </form>
        )}

        <div className={styles.switch}>
          <Link to="/login"><ArrowLeft size={14} /> Вернуться к входу</Link>
        </div>
      </div>
    </div>
  );
}
