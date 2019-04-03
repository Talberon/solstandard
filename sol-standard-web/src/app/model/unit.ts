export class Unit {
    readonly id: number;
    readonly name: string;
    readonly role: string;
    readonly description: string;

    constructor(id: number, name: string, role: string, description: string) {
        this.name = name;
        this.role = role;
        this.description = description;
    }

    getPortrait(team: Team): string {
        return `assets/images/media/portraits/${team.toString().toLowerCase()}/${this.name}.png`;
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