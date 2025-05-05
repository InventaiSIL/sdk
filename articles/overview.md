# Overview

The Inventai SDK is a powerful toolkit for creating interactive narrative experiences. It provides a comprehensive set of tools for generating and managing interactive stories, characters, and dialogue.

## Key Components

### Novel Generation
The SDK includes a robust novel generation system that can create branching narratives with multiple paths and endings. The `Novel` class serves as the core component for managing the story structure.

### Character Management
Character interactions are handled through the `CharacterManager` class, which manages character states, relationships, and dialogue options. Characters can have complex personalities and relationships that evolve throughout the story.

### Discussion Context
The `DiscussionContextManager` class maintains the context of ongoing conversations, ensuring continuity and coherence in character interactions. It tracks conversation history and manages the flow of dialogue.

### Export Options
The SDK supports multiple export formats, including:
- Ren'Py script format for visual novel engines
- JSON format for data storage and transfer
- Custom formats through the exporter interface

## Architecture

The SDK follows a modular architecture with clear separation of concerns:
- Core functionality in the `Inventai` project
- Novel-specific features in the `InventaiNovel` project
- Example implementations in the `InventaiNovelTestApp` project

## Getting Started

To begin using the SDK:
1. Install the required NuGet packages
2. Reference the Inventai and InventaiNovel projects
3. Create a new instance of the `Novel` class
4. Configure your characters and story parameters
5. Generate and export your narrative

For detailed instructions, see the [Quick Start](quickstart.md) guide. 