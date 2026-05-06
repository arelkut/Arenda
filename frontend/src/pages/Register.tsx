import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { UserPlus, Eye, EyeOff, Home } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import styles from './Auth.module.css';

export default function Register() {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPwd, setConfirmPwd] = useState('');
  const [showPwd, setShowPwd] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (password !== confirmPwd) {
      setError('Пароли не совпадают');
      return;
    }
    setLoading(true);
    try {
      await register({ email, password, first_name: firstName, last_name: lastName, phone });
      navigate('/');
    } catch (err: any) {
      setError(err.message || 'Ошибка регистрации');
    }
    setLoading(false);
  };

  return (
    <div className={styles.page}>
      <div className={styles.card}>
        <Link to="/" className={styles.logo}>
          <Home size={28} />
          <span>Недвижимость.РФ</span>
        </Link>
        <h2 className={styles.title}>Регистрация</h2>
        <p className={styles.subtitle}>Создайте аккаунт для доступа ко всем функциям</p>

        {error && <div className={styles.error}>{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className={styles.formRow}>
            <div className={styles.formGroup}>
              <label className="input-label">Имя</label>
              <input className="input-field" placeholder="Иван" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
            </div>
            <div className={styles.formGroup}>
              <label className="input-label">Фамилия</label>
              <input className="input-field" placeholder="Иванов" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
            </div>
          </div>
          <div className={styles.formGroup}>
            <label className="input-label">Email</label>
            <input className="input-field" type="email" placeholder="your@email.com" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div className={styles.formGroup}>
            <label className="input-label">Телефон</label>
            <input className="input-field" type="tel" placeholder="+7 (900) 123-45-67" value={phone} onChange={(e) => setPhone(e.target.value)} />
          </div>
          <div className={styles.formGroup}>
            <label className="input-label">Пароль</label>
            <div className={styles.passwordWrap}>
              <input
                className="input-field"
                type={showPwd ? 'text' : 'password'}
                placeholder="Минимум 6 символов"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                minLength={6}
              />
              <button type="button" className={styles.eyeBtn} onClick={() => setShowPwd(!showPwd)}>
                {showPwd ? <EyeOff size={18} /> : <Eye size={18} />}
              </button>
            </div>
          </div>
          <div className={styles.formGroup}>
            <label className="input-label">Подтвердите пароль</label>
            <input className="input-field" type="password" placeholder="Повторите пароль" value={confirmPwd} onChange={(e) => setConfirmPwd(e.target.value)} required />
          </div>
          <button type="submit" className={`btn btn-primary btn-lg ${styles.submitBtn}`} disabled={loading}>
            <UserPlus size={18} /> {loading ? 'Регистрация...' : 'Зарегистрироваться'}
          </button>
        </form>

        <div className={styles.switch}>
          Уже есть аккаунт? <Link to="/login">Войти</Link>
        </div>
      </div>
    </div>
  );
}
