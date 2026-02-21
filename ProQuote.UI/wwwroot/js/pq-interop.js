window.pqInterop = {
  setTheme: function (isDark) {
    const root = document.documentElement;
    if (isDark) {
      root.setAttribute('data-theme', 'dark');
      localStorage.setItem('pq-theme', 'dark');
    } else {
      root.removeAttribute('data-theme');
      localStorage.setItem('pq-theme', 'light');
    }
  },
  getTheme: function () {
    return localStorage.getItem('pq-theme') || 'light';
  }
};
