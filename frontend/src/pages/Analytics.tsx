import { TrendingUp, TrendingDown, BarChart3, PieChart, Activity, MapPin } from 'lucide-react';
import styles from './Analytics.module.css';

const cityData = [
  { city: 'Москва', avgPrice: 85000, change: 5.2, listings: 12500 },
  { city: 'Санкт-Петербург', avgPrice: 55000, change: 3.8, listings: 7800 },
  { city: 'Новосибирск', avgPrice: 28000, change: -1.2, listings: 3200 },
  { city: 'Екатеринбург', avgPrice: 30000, change: 2.1, listings: 2900 },
  { city: 'Казань', avgPrice: 32000, change: 4.5, listings: 2100 },
  { city: 'Краснодар', avgPrice: 35000, change: 6.3, listings: 3400 },
];

const typeStats = [
  { type: 'Квартиры', pct: 62, count: 35200 },
  { type: 'Студии', pct: 18, count: 10200 },
  { type: 'Дома', pct: 12, count: 6800 },
  { type: 'Коммерческая', pct: 8, count: 4500 },
];

export default function Analytics() {
  return (
    <div className={styles.page}>
      <div className="container">
        <h1 className={styles.title}>Аналитика рынка</h1>
        <p className={styles.subtitle}>Статистика и тренды рынка аренды недвижимости</p>

        {/* Summary Cards */}
        <div className={styles.summaryGrid}>
          <div className={styles.summaryCard}>
            <div className={styles.summaryIcon}><Activity size={24} /></div>
            <div className={styles.summaryValue}>56 700</div>
            <div className={styles.summaryLabel}>Активных объявлений</div>
            <div className={`${styles.summaryChange} ${styles.up}`}>
              <TrendingUp size={14} /> +8.5% за месяц
            </div>
          </div>
          <div className={styles.summaryCard}>
            <div className={styles.summaryIcon}><BarChart3 size={24} /></div>
            <div className={styles.summaryValue}>58 500 ₽</div>
            <div className={styles.summaryLabel}>Средняя цена аренды</div>
            <div className={`${styles.summaryChange} ${styles.up}`}>
              <TrendingUp size={14} /> +3.2% за месяц
            </div>
          </div>
          <div className={styles.summaryCard}>
            <div className={styles.summaryIcon}><PieChart size={24} /></div>
            <div className={styles.summaryValue}>14 дней</div>
            <div className={styles.summaryLabel}>Среднее время аренды</div>
            <div className={`${styles.summaryChange} ${styles.down}`}>
              <TrendingDown size={14} /> -2.1 дня за месяц
            </div>
          </div>
          <div className={styles.summaryCard}>
            <div className={styles.summaryIcon}><MapPin size={24} /></div>
            <div className={styles.summaryValue}>42</div>
            <div className={styles.summaryLabel}>Городов в базе</div>
            <div className={`${styles.summaryChange} ${styles.up}`}>
              <TrendingUp size={14} /> +3 за месяц
            </div>
          </div>
        </div>

        {/* City Stats Table */}
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>Цены по городам</h2>
          <div className={styles.tableWrap}>
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>Город</th>
                  <th>Средняя цена</th>
                  <th>Изменение</th>
                  <th>Объявлений</th>
                </tr>
              </thead>
              <tbody>
                {cityData.map((c) => (
                  <tr key={c.city}>
                    <td><strong>{c.city}</strong></td>
                    <td>{c.avgPrice.toLocaleString('ru-RU')} ₽/мес</td>
                    <td>
                      <span className={c.change > 0 ? styles.up : styles.down}>
                        {c.change > 0 ? <TrendingUp size={14} /> : <TrendingDown size={14} />}
                        {c.change > 0 ? '+' : ''}{c.change}%
                      </span>
                    </td>
                    <td>{c.listings.toLocaleString('ru-RU')}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>

        {/* Type Distribution */}
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>Распределение по типам</h2>
          <div className={styles.typeGrid}>
            {typeStats.map((t) => (
              <div key={t.type} className={styles.typeCard}>
                <div className={styles.typeHeader}>
                  <span className={styles.typeName}>{t.type}</span>
                  <span className={styles.typePct}>{t.pct}%</span>
                </div>
                <div className={styles.typeBar}>
                  <div className={styles.typeBarFill} style={{ width: `${t.pct}%` }}></div>
                </div>
                <div className={styles.typeCount}>{t.count.toLocaleString('ru-RU')} объявлений</div>
              </div>
            ))}
          </div>
        </section>
      </div>
    </div>
  );
}
