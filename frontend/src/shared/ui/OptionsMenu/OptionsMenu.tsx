"use client";

import { ReactNode, useEffect, useRef, useState } from "react";
import { HiDotsVertical } from "react-icons/hi";
import { twMerge } from "tailwind-merge";

export type MenuOption = {
  id?: string;
  label: string;
  onSelect: () => void | Promise<void>;
  icon?: ReactNode;
  disabled?: boolean;
  isDestructive?: boolean;
};

type OptionsMenuProps = {
  options: MenuOption[];
  buttonAriaLabel?: string;
  align?: "left" | "right";
  className?: string;
  disabled?: boolean;
  triggerContent?: ReactNode;
  iconSize?: number;
};

const OptionsMenu = ({
  options,
  buttonAriaLabel = "Open options menu",
  align = "right",
  className,
  disabled = false,
  triggerContent,
  iconSize = 20,
}: OptionsMenuProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!isOpen) return;

    const handleOutsideClick = (event: MouseEvent) => {
      if (
        menuRef.current &&
        !menuRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [isOpen]);

  const toggleMenu = () => {
    if (disabled) return;
    setIsOpen((prev) => !prev);
  };

  const handleSelect = async (option: MenuOption) => {
    if (option.disabled) return;
    await option.onSelect();
    setIsOpen(false);
  };

  return (
    <div
      className={twMerge("relative inline-block text-left", className)}
      ref={menuRef}
    >
      <button
        type="button"
        aria-haspopup="menu"
        aria-expanded={isOpen}
        aria-label={buttonAriaLabel}
        onClick={toggleMenu}
        disabled={disabled}
        className={twMerge(
          "flex items-center justify-center group p-2 rounded-full hover:bg-neutral-800/60 transition cursor-pointer",
          disabled ? "opacity-50 cursor-not-allowed" : ""
        )}
      >
        {triggerContent ?? (
          <HiDotsVertical
            size={iconSize}
            className="text-neutral-400 group-focus:text-neutral-200"
          />
        )}
      </button>

      {isOpen && (
        <div
          role="menu"
          className={twMerge(
            "absolute z-30 mt-2 w-50 origin-top-right" +
              "rounded-md bg-neutral-900/95 backdrop-blur ring-1 ring-black/30 shadow-lg focus:outline-none",
            align === "right" ? "right-0" : "left-0"
          )}
        >
          <div className="py-1">
            {options.map((option, index) => (
              <button
                key={option.id ?? `${option.label}-${index}`}
                role="menuitem"
                disabled={option.disabled}
                onClick={() => handleSelect(option)}
                className={twMerge(
                  "w-full px-2 py-2 text-left text-sm flex items-center gap-2 transition",
                  option.disabled
                    ? "text-neutral-500 cursor-not-allowed"
                    : option.isDestructive
                      ? "text-red-400 hover:bg-red-500/10"
                      : "text-neutral-200 hover:bg-neutral-800/70"
                )}
              >
                {option.icon && <span className="text-base">{option.icon}</span>}
                {option.label}
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default OptionsMenu;

