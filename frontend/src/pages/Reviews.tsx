import { Star, Quote } from 'lucide-react';
import styles from './Reviews.module.css';

const reviews = [
  {
    id: 1, name: 'Мария Иванова', role: 'Арендатор',
    text: 'Отличный сервис! Нашли квартиру мечты за неделю. Менеджер помог с документами и проверкой квартиры. Всё прозрачно и быстро.',
    rating: 5, date: 'Март 2024',
  },
  {
    id: 2, name: 'Алексей Козлов', role: 'Арендодатель',
    text: 'Как арендодатель, пользуюсь платформой уже 2 года. Удобный личный кабинет, быстрая модерация объявлений, отличная база потенциальных арендаторов.',
    rating: 5, date: 'Февраль 2024',
  },
  {
    id: 3, name: 'Елена Смирнова', role: 'Арендатор',
    text: 'Переехали в другой город и благодаря этому сервису нашли жильё ещё до переезда. Очень удобные фильтры и подробные описания объектов.',
    rating: 4, date: 'Февраль 2024',
  },
  {
    id: 4, name: 'Дмитрий Петров', role: 'Арендатор',
    text: 'Калькулятор ипотеки очень помог определиться с бюджетом. В итоге взяли ипотеку через партнёрский банк платформы с отличной ставкой.',
    rating: 5, date: 'Январь 2024',
  },
  {
    id: 5, name: 'Ольга Николаева', role: 'Арендодатель',
    text: 'Сдаю несколько квартир через эту платформу. Очень удобно управлять всеми объектами в одном месте. Арендаторы находятся быстро.',
    rating: 5, date: 'Январь 2024',
  },
  {
    id: 6, name: 'Сергей Волков', role: 'Арендатор',
    text: 'Понравилась аналитика рынка — помогла понять, в каком районе лучше искать. Цены адекватные, выбор большой.',
    rating: 4, date: 'Декабрь 2023',
  },
];

const avgRating = (reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1);

export default function Reviews() {
  return (
    <div className={styles.page}>
      <div className="container">
        <h1 className={styles.title}>Отзывы клиентов</h1>
        <p className={styles.subtitle}>Что говорят о нас наши пользователи</p>

        {/* Summary */}
        <div className={styles.summary}>
          <div className={styles.summaryRating}>
            <div className={styles.ratingNum}>{avgRating}</div>
            <div className={styles.ratingStars}>
              {[1, 2, 3, 4, 5].map((s) => (
                <Star key={s} size={24} fill={s <= Math.round(Number(avgRating)) ? '#f59e0b' : 'none'} color="#f59e0b" />
              ))}
            </div>
            <div className={styles.ratingCount}>На основе {reviews.length} отзывов</div>
          </div>
          <div className={styles.ratingBars}>
            {[5, 4, 3, 2, 1].map((star) => {
              const count = reviews.filter((r) => r.rating === star).length;
              const pct = (count / reviews.length) * 100;
              return (
                <div key={star} className={styles.barRow}>
                  <span>{star}</span>
                  <Star size={14} fill="#f59e0b" color="#f59e0b" />
                  <div className={styles.bar}>
                    <div className={styles.barFill} style={{ width: `${pct}%` }}></div>
                  </div>
                  <span className={styles.barCount}>{count}</span>
                </div>
              );
            })}
          </div>
        </div>

        {/* Reviews Grid */}
        <div className={styles.grid}>
          {reviews.map((r) => (
            <div key={r.id} className={styles.reviewCard}>
              <div className={styles.quoteIcon}><Quote size={24} /></div>
              <p className={styles.reviewText}>{r.text}</p>
              <div className={styles.reviewStars}>
                {[1, 2, 3, 4, 5].map((s) => (
                  <Star key={s} size={16} fill={s <= r.rating ? '#f59e0b' : 'none'} color="#f59e0b" />
                ))}
              </div>
              <div className={styles.reviewAuthor}>
                <div className={styles.avatar}>{r.name.charAt(0)}</div>
                <div>
                  <div className={styles.authorName}>{r.name}</div>
                  <div className={styles.authorRole}>{r.role} &middot; {r.date}</div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
