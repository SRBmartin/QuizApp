const apiBase = process.env.REACT_APP_API_BASE;

export const API_BASE = apiBase && apiBase.trim().length > 0
  ? apiBase
  : "http://localhost:5231";