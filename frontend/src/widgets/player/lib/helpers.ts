export function formatTime(timeInSeconds: number | undefined): string {
  if (!timeInSeconds || isNaN(timeInSeconds)) return "0:00";
  const minutes = Math.floor(timeInSeconds / 60);
  const seconds = Math.floor(timeInSeconds % 60);
  return `${minutes}:${seconds < 10 ? "0" : ""}${seconds}`;
}

export function calculateProgress(current: number, duration: number): number {
  return duration > 0 ? current / duration : 0;
}