function ExplicitBadge() {
  return (
    <span
      className="
        flex shrink-0 items-center justify-center w-4 h-4
        bg-neutral-400 text-neutral-900 text-xs font-bold rounded-xs
      "
      title="Explicit Content"
      aria-label="Explicit Content"
    >
      E
    </span>
  );
}

export default ExplicitBadge;