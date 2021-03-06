namespace GameplayPrompts {
    export interface UserPrompt {
      id: string;
      gameIdString: string;
      lobbyId: string;
      refreshTimeInMs: number;
      currentServerTime: Date;
      autoSubmitAtTime: Date;
      submitButton: boolean;
      submitButtonText: string;
      suggestion: SuggestionMetadata;
      title: string;
      description: string;
      subPrompts: SubPrompt[];
      error: string;
      tutorial: TutorialMetadata;
    }
    export interface SubPrompt {
      id: string;
      prompt: string;
      color: string;
      stringList: string[];
      dropdown: string[];    
      answers: string[];
      colorPicker: boolean;
      shortAnswer: boolean;
      drawing: DrawingPromptMetadata;
      slider: SliderPromptMetadata;
      selector: SelectorPromptMetadata;
    }
    export interface DrawingPromptMetadata {
      colorList: string[];
      widthInPx: number;
      heightInPx: number;
      premadeDrawing: string;
      canvasBackground: string;
      saveWithBackground: boolean;
      localStorageId: string;
    }
    export interface SliderPromptMetadata {
      min: number;
      max: number;
      value: string;
      ticks: number[];
      range: boolean;
      enabled: boolean;
      ticksLabels: string[];
      rangeHighlights: RangeHighlightsType[];
    }
    export interface SelectorPromptMetadata {
      widthInPx: number;
      heightInPx: number;
      imageList: string[];
    }
    export interface RangeHighlightsType
    {
        start: number;
        end: number;
        class: string; 
    }    
    export interface SuggestionMetadata {
      suggestionKey: string;
    }

    export interface TutorialMetadata {
      hideClasses: string[];
    }
}

export default GameplayPrompts