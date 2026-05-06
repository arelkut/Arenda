import { useState, useMemo } from 'react';
import { Calculator, Percent, Clock, Banknote } from 'lucide-react';
import styles from './Mortgage.module.css';

export default function Mortgage() {
  const [amount, setAmount] = useState(5000000);
  const [rate, setRate] = useState(12);
  const [years, setYears] = useState(20);
  const [downPayment, setDownPayment] = useState(1000000);

  const result = useMemo(() => {
    const principal = amount - downPayment;
    const monthlyRate = rate / 100 / 12;
    const months = years * 12;
    if (monthlyRate === 0) {
      return { monthly: principal / months, total: principal, overpayment: 0 };
    }
    const monthly = principal * (monthlyRate * Math.pow(1 + monthlyRate, months)) / (Math.pow(1 + monthlyRate, months) - 1);
    const total = monthly * months;
    const overpayment = total - principal;
    return { monthly, total, overpayment };
  }, [amount, rate, years, downPayment]);

  const fmt = (n: number) => Math.round(n).toLocaleString('ru-RU');

  const bankPrograms = [
    { name: 'Сбербанк', program: 'Ипотека на вторичное жильё', rate: '11.9%', term: 'до 30 лет', min: 'от 300 000 ₽' },
    { name: 'ВТБ', program: 'Ипотека для семей', rate: '6.0%', term: 'до 30 лет', min: 'от 500 000 ₽' },
    { name: 'Альфа-Банк', program: 'Готовое жильё', rate: '12.5%', term: 'до 30 лет', min: 'от 600 000 ₽' },
    { name: 'Газпромбанк', program: 'Льготная ипотека', rate: '8.0%', term: 'до 20 лет', min: 'от 500 000 ₽' },
    { name: 'Тинькофф', program: 'Ипотека онлайн', rate: '11.5%', term: 'до 25 лет', min: 'от 300 000 ₽' },
    { name: 'Россельхозбанк', program: 'Сельская ипотека', rate: '3.0%', term: 'до 25 лет', min: 'от 100 000 ₽' },
  ];

  return (
    <div className={styles.page}>
      <div className="container">
        <h1 className={styles.title}>Ипотечный калькулятор</h1>
        <p className={styles.subtitle}>Рассчитайте ежемесячный платёж и выберите лучшую программу</p>

        <div className={styles.calcGrid}>
          {/* Calculator */}
          <div className={styles.calcForm}>
            <div className={styles.formGroup}>
              <label className="input-label">Стоимость недвижимости</label>
              <input className="input-field" type="number" value={amount} onChange={(e) => setAmount(Number(e.target.value))} />
              <input type="range" min={500000} max={50000000} step={100000} value={amount} onChange={(e) => setAmount(Number(e.target.value))} className={styles.slider} />
              <div className={styles.rangeLabels}><span>500 тыс</span><span>50 млн</span></div>
            </div>

            <div className={styles.formGroup}>
              <label className="input-label">Первоначальный взнос</label>
              <input className="input-field" type="number" value={downPayment} onChange={(e) => setDownPayment(Number(e.target.value))} />
              <input type="range" min={0} max={amount * 0.9} step={100000} value={downPayment} onChange={(e) => setDownPayment(Number(e.target.value))} className={styles.slider} />
            </div>

            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label className="input-label">Срок (лет)</label>
                <input className="input-field" type="number" value={years} onChange={(e) => setYears(Number(e.target.value))} />
                <input type="range" min={1} max={30} value={years} onChange={(e) => setYears(Number(e.target.value))} className={styles.slider} />
              </div>
              <div className={styles.formGroup}>
                <label className="input-label">Ставка (%)</label>
                <input className="input-field" type="number" step={0.1} value={rate} onChange={(e) => setRate(Number(e.target.value))} />
                <input type="range" min={1} max={30} step={0.1} value={rate} onChange={(e) => setRate(Number(e.target.value))} className={styles.slider} />
              </div>
            </div>
          </div>

          {/* Results */}
          <div className={styles.results}>
            <div className={styles.resultCard}>
              <div className={styles.resultIcon}><Banknote size={24} /></div>
              <div>
                <div className={styles.resultLabel}>Ежемесячный платёж</div>
                <div className={styles.resultValue}>{fmt(result.monthly)} ₽</div>
              </div>
            </div>
            <div className={styles.resultCard}>
              <div className={styles.resultIcon}><Calculator size={24} /></div>
              <div>
                <div className={styles.resultLabel}>Сумма кредита</div>
                <div className={styles.resultValueSm}>{fmt(amount - downPayment)} ₽</div>
              </div>
            </div>
            <div className={styles.resultCard}>
              <div className={styles.resultIcon}><Clock size={24} /></div>
              <div>
                <div className={styles.resultLabel}>Общая сумма выплат</div>
                <div className={styles.resultValueSm}>{fmt(result.total)} ₽</div>
              </div>
            </div>
            <div className={styles.resultCard}>
              <div className={styles.resultIcon}><Percent size={24} /></div>
              <div>
                <div className={styles.resultLabel}>Переплата</div>
                <div className={styles.resultValueSm}>{fmt(result.overpayment)} ₽</div>
              </div>
            </div>
          </div>
        </div>

        {/* Bank Programs */}
        <section className={styles.banks}>
          <h2 className="section-title">Предложения банков</h2>
          <p className="section-subtitle">Актуальные ипотечные программы ведущих банков</p>
          <div className={styles.bankGrid}>
            {bankPrograms.map((b) => (
              <div key={b.name} className={styles.bankCard}>
                <div className={styles.bankName}>{b.name}</div>
                <div className={styles.bankProgram}>{b.program}</div>
                <div className={styles.bankDetails}>
                  <div><span>Ставка</span><strong>{b.rate}</strong></div>
                  <div><span>Срок</span><strong>{b.term}</strong></div>
                  <div><span>Мин. сумма</span><strong>{b.min}</strong></div>
                </div>
                <button className="btn btn-outline" style={{ width: '100%' }}>Подробнее</button>
              </div>
            ))}
          </div>
        </section>
      </div>
    </div>
  );
}
