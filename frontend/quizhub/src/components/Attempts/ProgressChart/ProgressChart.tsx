import React, { useEffect, useMemo, useRef, useState } from "react";
import "./ProgressChart.scss";
import { attemptsApi } from "../../../services/attempts.api";
import type { MyAttempt } from "../../../models/my-attempt";
import type { Paged } from "../../../models/pages";

type Props = {
  quizId: string;
  currentAttemptId?: string;
};

const HEIGHT = 180;
const PADDING = 16;

const ProgressChart: React.FC<Props> = ({ quizId, currentAttemptId }) => {
  const [pts, setPts] = useState<{ id: string; date: Date; pct: number }[]>([]);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const rootRef = useRef<HTMLDivElement | null>(null);
  const [width, setWidth] = useState(380);

  // keep only newest response
  const seqRef = useRef(0);

  // observe container width
  useEffect(() => {
    if (!rootRef.current) return;
    const el = rootRef.current;
    const ro = new ResizeObserver(entries => {
      for (const e of entries) {
        if (e.contentRect.width > 0) setWidth(e.contentRect.width);
      }
    });
    ro.observe(el);
    return () => ro.disconnect();
  }, []);

  // fetch attempts (no caching that blocks later mounts)
  useEffect(() => {
    if (!quizId) return;
    const mySeq = ++seqRef.current;
    let cancelled = false;

    (async () => {
      setLoading(true);
      setErr(null);
      try {
        const resp: Paged<MyAttempt> = await attemptsApi.myAttempts(0, 100, {
          status: "Completed",
          quizId,
        });

        if (cancelled || mySeq !== seqRef.current) return;

        const data = (resp.items ?? [])
          .filter(a => a.submittedAt)
          .map(a => ({
            id: a.attemptId,
            date: new Date(a.submittedAt as string),
            pct: Math.max(
              0,
              Math.min(
                100,
                Math.round(
                  a.percentage ?? ((a.maxScore ?? 0) > 0 ? (a.totalScore / a.maxScore) * 100 : 0)
                )
              )
            )
          }))
          .sort((a, b) => +a.date - +b.date);

        setPts(data);
      } catch (e: any) {
        if (!cancelled && mySeq === seqRef.current) setErr(e?.message ?? "Failed to load progress.");
      } finally {
        if (!cancelled && mySeq === seqRef.current) setLoading(false);
      }
    })();

    return () => { cancelled = true; };
  }, [quizId]);

  const hasEnough = pts.length >= 2;

  const { pathD, circles, yTicks } = useMemo(() => {
    if (!hasEnough) return { pathD: "", circles: [], yTicks: [0, 25, 50, 75, 100] };
    const w = Math.max(240, width);
    const h = HEIGHT, innerW = w - PADDING * 2, innerH = h - PADDING * 2;
    const xs = (i: number) => (pts.length === 1 ? PADDING + innerW / 2 : PADDING + (i * innerW) / (pts.length - 1));
    const ys = (pct: number) => Math.min(h - PADDING, Math.max(PADDING, PADDING + innerH * (1 - pct / 100)));
    const d = pts.map((p, i) => `${i === 0 ? "M" : "L"} ${xs(i)} ${ys(p.pct)}`).join(" ");
    const circles = pts.map((p, i) => ({
      cx: xs(i), cy: ys(p.pct), id: p.id, pct: p.pct,
      dateLabel: p.date.toLocaleString(),
      isCurrent: currentAttemptId === p.id
    }));
    const yTicks = [0, 25, 50, 75, 100];
    return { pathD: d, circles, yTicks };
  }, [pts, width, currentAttemptId, hasEnough]);

  return (
    <div className="progress-embed" ref={rootRef}>
      <div className="pg-head">
        <h3 className="pg-title">Progress over attempts</h3>
        {hasEnough && <span className="pill">{pts.length} completed</span>}
      </div>

      {/* states */}
      {loading && !pts.length && <div className="muted">Loading…</div>}
      {err && !pts.length && <div className="error">{err}</div>}
      {!loading && !err && !hasEnough && (
        <div className="muted">Complete more attempts to see your progress.</div>
      )}

      {/* chart */}
      {hasEnough && (
        <div className="pg-chart">
          <svg width="100%" height={HEIGHT} viewBox={`0 0 ${Math.max(240, width)} ${HEIGHT}`} role="img" aria-label="Progress line chart">
            {yTicks.map((t) => {
              const y = PADDING + (HEIGHT - PADDING * 2) * (1 - t / 100);
              return (
                <g key={t}>
                  <line x1={PADDING} x2={Math.max(240, width) - PADDING} y1={y} y2={y} className="grid" />
                  <text x={PADDING - 6} y={y} alignmentBaseline="middle" textAnchor="end" className="tick">{t}%</text>
                </g>
              );
            })}
            <path d={pathD} className="line" />
            {circles.map((c, i) => (
              <g key={c.id}>
                <circle cx={c.cx} cy={c.cy} r={c.isCurrent ? 5 : 4} className={c.isCurrent ? "pt current" : "pt"} />
                <title>{`#${i + 1} — ${c.dateLabel} • ${c.pct}%`}{c.isCurrent ? " (this attempt)" : ""}</title>
              </g>
            ))}
          </svg>
        </div>
      )}

      {hasEnough && (
        <div className="pg-legend">
          <span className="dot current" /> Current attempt
          <span className="dot other" /> Previous attempts
        </div>
      )}
    </div>
  );
};

export default ProgressChart;
