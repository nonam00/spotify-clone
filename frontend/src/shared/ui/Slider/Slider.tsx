"use client";

import {memo, useState} from "react";
import * as RadixSlider from "@radix-ui/react-slider";
import {twMerge} from "tailwind-merge";

type CustomSliderProps = RadixSlider.SliderProps & { isLoading?: boolean };

const Slider = memo(function Slider({
  value = [1],
  onValueCommit,
  isLoading,
  disabled,
}: CustomSliderProps) {
  const [sliderValue, setValue] = useState(value);
  return (
    <RadixSlider.Root
      className="relative flex items-center select-none touch-none w-full h-7 cursor-pointer"
      defaultValue={[1]}
      value={sliderValue}
      onValueChange={setValue}
      onValueCommit={onValueCommit}
      max={1}
      step={0.01}
      disabled={disabled}
    >
      <RadixSlider.Track
        className={twMerge(
          "bg-neutral-600 relative grow rounded-full h-[3.1px]",
          isLoading ? "animate-pulse" : ""
        )}>
        <RadixSlider.Range className="absolute bg-white rounded-full h-full" />
      </RadixSlider.Track>
    </RadixSlider.Root>
  );
});

export default Slider;

