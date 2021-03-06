//
//  ViewController.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-15.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "ViewController.h"
#import "EditorView.h"
#import "LinkedNote.h"
#import "PrototypeView.h"

@interface ViewController()
-(NSString *)retrieveUuidOfFirstEditorDocument;
@end

@implementation ViewController
@synthesize mainTableView;

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Release any cached data, images, etc that aren't in use.
}


#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
}

- (void)viewDidUnload
{
    mainTableView = nil;
    [self setMainTableView:nil];
    [super viewDidUnload];
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
}

- (void)viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
}

- (void)viewWillDisappear:(BOOL)animated
{
	[super viewWillDisappear:animated];
}

- (void)viewDidDisappear:(BOOL)animated
{
	[super viewDidDisappear:animated];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone) {
        return (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown);
    } else {
        return YES;
    }
}
#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return 2;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
    }
    switch (indexPath.row) 
    {
        case 0:
        {
            cell.textLabel.text = @"Editor";
        }
            break;
        case 1:
        {
            cell.textLabel.text = @"Prototype";
        }
            break;
        default:
            break;
    }
    
    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    // Navigation logic may go here. Create and push another view controller.
    /*
     <#DetailViewController#> *detailViewController = [[<#DetailViewController#> alloc] initWithNibName:@"<#Nib name#>" bundle:nil];
     // ...
     // Pass the selected object to the new view controller.
     [self.navigationController pushViewController:detailViewController animated:YES];
     */
    
    switch (indexPath.row) 
    {
        case 0:
        {
            EditorView *view;
            if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone) 
            {
                view = [[EditorView alloc] initWithNibName:@"EditorView_iPhone" bundle:nil]; 
            } 
            else 
            {
                view = [[EditorView alloc] initWithNibName:@"EditorView_iPad" bundle:nil]; 
            }
            NSString *documentUuid = [self retrieveUuidOfFirstEditorDocument];
            [view assignDocumentUuid:documentUuid];
            [self.navigationController pushViewController:view animated:YES];

            break;
        }
        case 1:
        {
            PrototypeView *view;
            if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone) 
            {
                //view = [[PrototypeView alloc] initWithNibName:@"PrototypeView_iPhone" bundle:nil]; 
            } 
            else 
            {
                view = [[PrototypeView alloc] initWithNibName:@"PrototypeView_iPad" bundle:nil]; 
            }
            [self.navigationController pushViewController:view animated:YES];

        }
        default:
            break;
    }
}

-(NSString *)retrieveUuidOfFirstEditorDocument
{
    NSMutableArray *array = [[DataController sharedInstance] allInstancesOf:@"EditorDocument" 
                                                                  orderedBy:nil 
                                                                   loadData:NO 
                                                                  targetMOC:nil];
    NSString *result = nil;
    if([array count] > 0)
    {
        LinkedNote *document = [array objectAtIndex:0];
        result = document.uuid;
    }
    return result;
}
@end
