//
//  SectionView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "SectionView.h"
#import "CategoryView.h"
#import "BDSection.h"
#import "BDCategory.h"

@implementation SectionView
@synthesize dataTableView;
@synthesize sectionArray;
@synthesize parentId;
@synthesize parentName;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName
{
    self = [super initWithNibName:@"SectionView" bundle:nil];
    if(self)
    {
        parentId = [pParentId retain];
        parentName = [pParentName retain];
    }
    return self;
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    self.title = self.parentName;
    [self.navigationController setNavigationBarHidden:NO animated:YES];
}

-(void)viewWillAppear:(BOOL)animated 
{
    [super viewWillAppear:animated];
    self.sectionArray = [NSArray arrayWithArray:[BDSection retrieveAllWithParentUUID:self.parentId]];
}

- (void)viewDidUnload
{
    [self setDataTableView:nil];
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)dealloc {
    [dataTableView release];
    [sectionArray release];
    [parentId release];
    [super dealloc];
}

#pragma mark - TableView DataSource

- (int)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.sectionArray count];
}

#pragma mark - TableView Delegate

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [self.dataTableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
    }
    cell.textLabel.text = [[self.sectionArray objectAtIndex:indexPath.row] name ];
    
    int childCount = [[BDCategory retrieveCountWithParentUUID:[[self.sectionArray objectAtIndex:indexPath.row] uuid]] intValue];
    cell.accessoryType = (childCount > 0) ? UITableViewCellAccessoryDisclosureIndicator : UITableViewCellAccessoryNone;
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    BDSection *section = [self.sectionArray objectAtIndex:indexPath.row];
     CategoryView *vwCategory = [[CategoryView alloc] initWithParentId:[section uuid] withParentName: [section name]];
     [self.navigationController pushViewController:vwCategory animated:YES];
     [vwCategory release];
}

@end
