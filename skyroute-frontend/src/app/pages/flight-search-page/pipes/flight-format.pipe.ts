export class FlightFormatUtils {
  static formatTime(iso: string, compareWith?: string): string {
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return '—';
    const time = new Intl.DateTimeFormat(undefined, { hour: '2-digit', minute: '2-digit' }).format(d);
    if (compareWith) {
      const ref = new Date(compareWith);
      if (!Number.isNaN(ref.getTime()) && d.toDateString() !== ref.toDateString()) {
        return `${time} +1d`;
      }
    }
    return time;
  }

  static formatDuration(minutes: number): string {
    if (minutes < 0 || !Number.isFinite(minutes)) return '—';
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    if (h <= 0) return `${m}m`;
    return m > 0 ? `${h}h ${m}m` : `${h}h`;
  }

  static formatPrice(amount: number, currency: string): string {
    const c = currency || 'USD';
    try {
      return new Intl.NumberFormat(undefined, { style: 'currency', currency: c }).format(amount);
    } catch {
      return `${amount} ${c}`;
    }
  }
}
