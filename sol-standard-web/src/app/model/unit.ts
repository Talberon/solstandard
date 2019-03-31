export class Unit {
    id: number;
    name: string;
    role: string;
    description: string;
    imagePath: string;

    constructor(id: number, name: string, role: string, description: string) {
        this.name = name;
        this.role = role;
        this.description = description;
        this.imagePath = `/assets/images/media/portraits/red/${name}.png`;
    }
}