export class Unit {
    readonly id: number;
    readonly name: string;
    readonly role: string;
    readonly description: string;

    constructor(id: number, name: string, role: string, description: string) {
        this.id = id;
        this.name = name;
        this.role = role;
        this.description = description;
    }

    getPortrait(team: Team): string {
        return `assets/images/media/units/portraits/${team.toString().toLowerCase()}/${this.name}.png`;
    }

    getGifs(): string[] {
        return [
            `assets/images/media/units/gifs/${this.name.toLowerCase()}_01.gif`,
            `assets/images/media/units/gifs/${this.name.toLowerCase()}_02.gif`,
            `assets/images/media/units/gifs/${this.name.toLowerCase()}_03.gif`,
        ];
    }
}

export enum Team {
    Red = 'Red',
    Blue = 'Blue'
}

export enum Role {
    Tank = 'Tank',
    Melee = 'Melee',
    Ranged = 'Ranged',
    Support = 'Support'
}