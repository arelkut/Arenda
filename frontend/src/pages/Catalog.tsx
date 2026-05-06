import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { Search, MapPin, SlidersHorizontal, X, ChevronLeft, ChevronRight } from 'lucide-react';
import { api } from '../services/api';
import type { Property } from '../services/api';
import styles from './Catalog.module.css';

export default function Catalog() {
  const [searchParams] = useSearchParams();
  const [properties, setProperties] = useState<Property[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [filtersOpen, setFiltersOpen] = useState(false);

  const [searchQuery, setSearchQuery] = useState(searchParams.get('search') || '');
  const [city, setCity] = useState(searchParams.get('city') || '');
  const [propertyType, setPropertyType] = useState(searchParams.get('type') || '');
  const [rooms, setRooms] = useState(searchParams.get('rooms') || '');
  const [minPrice, setMinPrice] = useState(searchParams.get('min_price') || '');
  const [maxPrice, setMaxPrice] = useState(searchParams.get('max_price') || '');

  useEffect(() => {
    fetchProperties();
  }, [page]);

  const fetchProperties = async () => {
    setLoading(true);
    const params: Record<string, string> = { page: String(page), size: '12' };
    if (searchQuery) params.search = searchQuery;
    if (city) params.city = city;
    if (propertyType) params.property_type = propertyType;
    if (rooms) params.rooms = rooms;
    if (minPrice) params.min_price = minPrice;
    if (maxPrice) params.max_price = maxPrice;
    try {
      const data = await api.getProperties(params);
      setProperties(data.items);
      setTotal(data.total);
    } catch {
      setProperties([]);
    }
    setLoading(false);
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    fetchProperties();
  };

  const resetFilters = () => {
    setSearchQuery('');
    setCity('');
    setPropertyType('');
    setRooms('');
    setMinPrice('');
    setMaxPrice('');
    setPage(1);
    setTimeout(fetchProperties, 0);
  };

  const totalPages = Math.ceil(total / 12);

  return (
    <div className={styles.page}>
      <div className="container">
        {/* Title */}
        <div className={styles.header}>
          <h1 className={styles.title}>Каталог недвижимости</h1>
          <p className={styles.subtitle}>Найдено {total} объявлений</p>
        </div>

        {/* Search Bar */}
        <form className={styles.searchBar} onSubmit={handleSearch}>
          <div className={styles.searchInput}>
            <Search size={20} />
            <input
              type="text"
              placeholder="Поиск по названию или адресу..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>
          <button type="button" className={styles.filterBtn} onClick={() => setFiltersOpen(!filtersOpen)}>
            <SlidersHorizontal size={18} /> Фильтры
          </button>
          <button type="submit" className="btn btn-primary">Найти</button>
        </form>

        {/* Filters Panel */}
        {filtersOpen && (
          <div className={styles.filters}>
            <div className={styles.filtersGrid}>
              <div className={styles.filterGroup}>
                <label className="input-label">Город</label>
                <input className="input-field" placeholder="Москва" value={city} onChange={(e) => setCity(e.target.value)} />
              </div>
              <div className={styles.filterGroup}>
                <label className="input-label">Тип</label>
                <select className="input-field" value={propertyType} onChange={(e) => setPropertyType(e.target.value)}>
                  <option value="">Все</option>
                  <option value="Квартира">Квартира</option>
                  <option value="Студия">Студия</option>
                  <option value="Дом">Дом</option>
                  <option value="Коммерческая">Коммерческая</option>
                </select>
              </div>
              <div className={styles.filterGroup}>
                <label className="input-label">Кол-во комнат</label>
                <select className="input-field" value={rooms} onChange={(e) => setRooms(e.target.value)}>
                  <option value="">Любое</option>
                  <option value="1">1</option>
                  <option value="2">2</option>
                  <option value="3">3</option>
                  <option value="4">4+</option>
                </select>
              </div>
              <div className={styles.filterGroup}>
                <label className="input-label">Цена от</label>
                <input className="input-field" type="number" placeholder="0" value={minPrice} onChange={(e) => setMinPrice(e.target.value)} />
              </div>
              <div className={styles.filterGroup}>
                <label className="input-label">Цена до</label>
                <input className="input-field" type="number" placeholder="999999" value={maxPrice} onChange={(e) => setMaxPrice(e.target.value)} />
              </div>
            </div>
            <div className={styles.filtersActions}>
              <button className="btn btn-primary" onClick={() => { setPage(1); fetchProperties(); }}>Применить</button>
              <button className="btn btn-outline" onClick={resetFilters}><X size={16} /> Сбросить</button>
            </div>
          </div>
        )}

        {/* Properties Grid */}
        {loading ? (
          <div className={styles.loading}>Загрузка...</div>
        ) : properties.length === 0 ? (
          <div className={styles.empty}>
            <p>Объявления не найдены</p>
            <button className="btn btn-outline" onClick={resetFilters}>Сбросить фильтры</button>
          </div>
        ) : (
          <>
            <div className={styles.grid}>
              {properties.map((p) => (
                <Link to={`/catalog/${p.property_id}`} key={p.property_id} className={`card ${styles.propCard}`}>
                  <div className={styles.propImage}>
                    <span className={styles.propBadge}>{p.property_type}</span>
                  </div>
                  <div className={styles.propBody}>
                    <h3 className={styles.propTitle}>{p.title}</h3>
                    <div className={styles.propAddress}><MapPin size={14} /> {p.city}, {p.address}</div>
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

            {/* Pagination */}
            {totalPages > 1 && (
              <div className={styles.pagination}>
                <button
                  className={styles.pageBtn}
                  disabled={page <= 1}
                  onClick={() => setPage(page - 1)}
                >
                  <ChevronLeft size={18} />
                </button>
                {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
                  <button
                    key={p}
                    className={`${styles.pageBtn} ${p === page ? styles.active : ''}`}
                    onClick={() => setPage(p)}
                  >
                    {p}
                  </button>
                ))}
                <button
                  className={styles.pageBtn}
                  disabled={page >= totalPages}
                  onClick={() => setPage(page + 1)}
                >
                  <ChevronRight size={18} />
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
