export class GameTile {
    readonly name: string;
    readonly description: string;

    constructor(name: string, description: string) {
        this.name = name;
        this.description = description;
    }

    getImageSrc(): string {
        return `assets/images/media/tiles/${this.name.toLowerCase()}.png`;
    }
}