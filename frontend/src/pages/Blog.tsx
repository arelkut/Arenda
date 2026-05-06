import { Clock, User, Tag, ArrowRight } from 'lucide-react';
import styles from './Blog.module.css';

const articles = [
  {
    id: 1,
    title: 'Как выбрать идеальную квартиру для аренды',
    excerpt: 'Подробное руководство по выбору квартиры: на что обратить внимание при осмотре, какие вопросы задать арендодателю, как проверить документы.',
    category: 'Советы',
    author: 'Анна Петрова',
    date: '15 марта 2024',
    readTime: '8 мин',
  },
  {
    id: 2,
    title: 'Тренды рынка недвижимости 2024',
    excerpt: 'Анализ текущих тенденций на рынке аренды: рост цен, популярные районы, прогнозы экспертов на ближайший год.',
    category: 'Аналитика',
    author: 'Михаил Сидоров',
    date: '12 марта 2024',
    readTime: '12 мин',
  },
  {
    id: 3,
    title: 'Ипотека в 2024: что изменилось',
    excerpt: 'Обзор новых ипотечных программ, изменения в ключевой ставке, льготные условия для разных категорий заёмщиков.',
    category: 'Ипотека',
    author: 'Елена Козлова',
    date: '10 марта 2024',
    readTime: '10 мин',
  },
  {
    id: 4,
    title: 'Права и обязанности арендатора',
    excerpt: 'Юридический ликбез: что нужно знать о своих правах и обязанностях при аренде жилья, как защитить свои интересы.',
    category: 'Право',
    author: 'Дмитрий Волков',
    date: '8 марта 2024',
    readTime: '15 мин',
  },
  {
    id: 5,
    title: 'Дизайн арендной квартиры: как сделать уютно',
    excerpt: 'Советы по обустройству арендованного жилья без капитального ремонта: декор, мебель, освещение.',
    category: 'Интерьер',
    author: 'Ольга Смирнова',
    date: '5 марта 2024',
    readTime: '7 мин',
  },
  {
    id: 6,
    title: 'Инвестиции в недвижимость для начинающих',
    excerpt: 'Как начать инвестировать в недвижимость с минимальным бюджетом: стратегии, риски, ожидаемая доходность.',
    category: 'Инвестиции',
    author: 'Алексей Новиков',
    date: '1 марта 2024',
    readTime: '11 мин',
  },
];

export default function Blog() {
  return (
    <div className={styles.page}>
      <div className="container">
        <h1 className={styles.title}>Блог о недвижимости</h1>
        <p className={styles.subtitle}>Полезные статьи, аналитика и советы экспертов</p>

        <div className={styles.grid}>
          {articles.map((a, i) => (
            <div key={a.id} className={`card ${styles.article} ${i === 0 ? styles.featured : ''}`}>
              <div className={styles.articleImage}>
                <span className={styles.category}><Tag size={12} /> {a.category}</span>
              </div>
              <div className={styles.articleBody}>
                <h3 className={styles.articleTitle}>{a.title}</h3>
                <p className={styles.articleExcerpt}>{a.excerpt}</p>
                <div className={styles.articleMeta}>
                  <span><User size={14} /> {a.author}</span>
                  <span><Clock size={14} /> {a.readTime}</span>
                  <span>{a.date}</span>
                </div>
                <button className={styles.readMore}>Читать далее <ArrowRight size={16} /></button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
