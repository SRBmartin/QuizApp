const TOKEN_KEY = "auth.token";
const EVT = "auth:changed";

export const authStorage = {
  get token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  },
  set token(value: string | null) {
    if (value) localStorage.setItem(TOKEN_KEY, value);
    else localStorage.removeItem(TOKEN_KEY);
    window.dispatchEvent(new CustomEvent(EVT));
  },
  onChange(handler: () => void) {
    const fn = () => handler();
    window.addEventListener(EVT, fn);
    
    const storageFn = (e: StorageEvent) => {
      if (e.key === TOKEN_KEY) handler();
    };
    window.addEventListener("storage", storageFn);
    return () => {
      window.removeEventListener(EVT, fn);
      window.removeEventListener("storage", storageFn);
    };
  },
  clear() {
    this.token = null;
  }
};

export { TOKEN_KEY, EVT };
