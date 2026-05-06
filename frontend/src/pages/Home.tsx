import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { BookOpen, Calculator, Star, MapPin, ArrowRight, Building2, Home as HomeIcon, Landmark, Users } from 'lucide-react';
import { api } from '../services/api';
import type { Property } from '../services/api';
import styles from './Home.module.css';

export default function Home() {
  const [properties, setProperties] = useState<Property[]>([]);

  useEffect(() => {
    api.getProperties({ size: '6' }).then((data) => setProperties(data.items)).catch(() => {});
  }, []);

  return (
    <div>
      {/* Hero */}
      <section className={styles.hero}>
        <div className={`container ${styles.heroInner}`}>
          <h1 className={styles.heroTitle}>Найдите свой идеальный дом</h1>
          <p className={styles.heroSubtitle}>
            Тысячи объектов недвижимости по всей России. Аренда квартир, домов, коммерческой недвижимости
          </p>
          <Link to="/catalog" className={`btn btn-accent btn-lg ${styles.heroBtn}`}>
            Начать поиск <ArrowRight size={20} />
          </Link>
        </div>
      </section>

      {/* Feature Cards */}
      <section className={`section ${styles.features}`}>
        <div className="container">
          <div className={styles.featGrid}>
            <div className={styles.featCard}>
              <div className={styles.featIcon}><BookOpen size={32} /></div>
              <h3>Полезные статьи</h3>
              <p>Экспертные материалы об ипотеке, ремонте и инвестициях в недвижимость</p>
              <Link to="/blog" className={styles.featLink}>Читать блог <ArrowRight size={16} /></Link>
            </div>
            <div className={styles.featCard}>
              <div className={styles.featIcon}><Calculator size={32} /></div>
              <h3>Калькулятор ипотеки</h3>
              <p>Рассчитайте ежемесячный платёж и подберите лучшую ипотечную программу</p>
              <Link to="/mortgage" className={styles.featLink}>Рассчитать <ArrowRight size={16} /></Link>
            </div>
            <div className={styles.featCard}>
              <div className={styles.featIcon}><Star size={32} /></div>
              <h3>Отзывы клиентов</h3>
              <p>Реальные истории людей, которые нашли свой дом с помощью нашего сервиса</p>
              <Link to="/reviews" className={styles.featLink}>Смотреть отзывы <ArrowRight size={16} /></Link>
            </div>
            <div className={styles.featCard}>
              <div className={styles.featIcon}><MapPin size={32} /></div>
              <h3>Интерактивная карта</h3>
              <p>Изучайте цены и доступность недвижимости в разных районах города</p>
              <Link to="/analytics" className={styles.featLink}>Открыть карту <ArrowRight size={16} /></Link>
            </div>
          </div>
        </div>
      </section>

      {/* Property Types */}
      <section className={styles.types}>
        <div className="container">
          <h2 className="section-title">Типы недвижимости</h2>
          <p className="section-subtitle">Выберите подходящий тип объекта для аренды</p>
          <div className={styles.typesGrid}>
            <Link to="/catalog?type=Квартира" className={styles.typeCard}>
              <Building2 size={40} />
              <h4>Квартиры</h4>
              <span>от 30 000 ₽/мес</span>
            </Link>
            <Link to="/catalog?type=Студия" className={styles.typeCard}>
              <HomeIcon size={40} />
              <h4>Студии</h4>
              <span>от 25 000 ₽/мес</span>
            </Link>
            <Link to="/catalog?type=Дом" className={styles.typeCard}>
              <Landmark size={40} />
              <h4>Дома</h4>
              <span>от 60 000 ₽/мес</span>
            </Link>
            <Link to="/catalog?type=Коммерческая" className={styles.typeCard}>
              <Users size={40} />
              <h4>Коммерческая</h4>
              <span>от 80 000 ₽/мес</span>
            </Link>
          </div>
        </div>
      </section>

      {/* Latest Properties */}
      <section className="section">
        <div className="container">
          <h2 className="section-title">Новые объявления</h2>
          <p className="section-subtitle">Свежие предложения аренды недвижимости</p>
          <div className={styles.propGrid}>
            {properties.map((p) => (
              <Link to={`/catalog/${p.property_id}`} key={p.property_id} className={`card ${styles.propCard}`}>
                <div className={styles.propImage}>
                  <span className={styles.propType}>{p.property_type}</span>
                </div>
                <div className={styles.propBody}>
                  <h4 className={styles.propTitle}>{p.title}</h4>
                  <div className={styles.propAddress}><MapPin size={14} /> {p.address}</div>
                  <div className={styles.propMeta}>
                    {p.rooms && <span>{p.rooms} комн.</span>}
                    <span>{p.area} м²</span>
                    {p.floor && <span>{p.floor}/{p.total_floors} эт.</span>}
                  </div>
                  <div className={styles.propPrice}>{Number(p.price).toLocaleString('ru-RU')} ₽/мес</div>
                </div>
              </Link>
            ))}
          </div>
          <div className={styles.propMore}>
            <Link to="/catalog" className="btn btn-outline btn-lg">Смотреть все объявления <ArrowRight size={18} /></Link>
          </div>
        </div>
      </section>

      {/* Stats */}
      <section className={styles.stats}>
        <div className="container">
          <div className={styles.statsGrid}>
            <div className={styles.statItem}>
              <div className={styles.statNum}>100K+</div>
              <div className={styles.statLabel}>Объектов в базе</div>
            </div>
            <div className={styles.statItem}>
              <div className={styles.statNum}>25K+</div>
              <div className={styles.statLabel}>Довольных клиентов</div>
            </div>
            <div className={styles.statItem}>
              <div className={styles.statNum}>15+</div>
              <div className={styles.statLabel}>Лет на рынке</div>
            </div>
            <div className={styles.statItem}>
              <div className={styles.statNum}>4.9</div>
              <div className={styles.statLabel}>Средний рейтинг</div>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
