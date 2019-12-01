export class Screenshot {
    readonly id: number;
    readonly fileName: string;
    readonly caption: string;

    constructor(id: number, fileName: string, caption: string) {
        this.id = id;
        this.fileName = fileName;
        this.caption = caption;
    }

    getImage(): string {
        return `assets/images/media/screenshots/${this.fileName}`;
    }
}